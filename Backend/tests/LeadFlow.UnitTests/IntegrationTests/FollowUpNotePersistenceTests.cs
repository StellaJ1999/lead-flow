using LeadFlow.Application.DTOs.Leads;
using LeadFlow.Domain.Entities;
using LeadFlow.Infrastructure.Identity.Data;
using LeadFlow.Infrastructure.Persistence.Repositories;
using LeadFlow.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LeadFlow.UnitTests.IntegrationTests;

public class FollowUpNotePersistenceTests
{
    [Theory]
    [InlineData("valid")]
    [InlineData("boundary")]
    [InlineData("empty")]
    [InlineData("whitespace")]
    [InlineData("too-long")]
    [InlineData("missing-lead")]
    [InlineData("missing-author")]
    public async Task AddFollowUpNoteAsync_SavesOnlyValidNotes(
        string scenario)
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppIdentityDbContext>()
            .UseSqlite(connection)
            .Options;

        var author = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "staff@example.com",
            Email = "staff@example.com"
        };

        var lead = new Lead(
            "Note test",
            "contact@example.com",
            null,
            "Please contact me.");

        await using (var setup = new AppIdentityDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();

            setup.Users.Add(author);
            setup.Leads.Add(lead);

            await setup.SaveChangesAsync();
        }

        var text = scenario switch
        {
            "boundary" => new string('a', 4000),
            "empty" => "",
            "whitespace" => "   ",
            "too-long" => new string('a', 4001),
            _ => "  Called the customer.  "
        };

        var requestedLeadId = scenario == "missing-lead"
            ? Guid.NewGuid()
            : lead.Id;

        var requestedAuthorId = scenario == "missing-author"
            ? Guid.NewGuid().ToString()
            : author.Id;

        FollowUpNoteResponse? result = null;
        var before = DateTimeOffset.UtcNow;

        await using (var write = new AppIdentityDbContext(options))
        {
            var repository = new LeadRepository(write);
            var service = new LeadService(repository);

            var request = new CreateFollowUpNoteRequest
            {
                Text = text
            };

            if (scenario is "empty" or "whitespace" or "too-long")
            {
                await Assert.ThrowsAsync<ArgumentException>(async () =>
                {
                    await service.AddFollowUpNoteAsync(
                        requestedLeadId,
                        request,
                        requestedAuthorId,
                        CancellationToken.None);
                });
            }
            else if (scenario == "missing-author")
            {
                await Assert.ThrowsAsync<UnauthorizedAccessException>(
                    async () =>
                    {
                        await service.AddFollowUpNoteAsync(
                            requestedLeadId,
                            request,
                            requestedAuthorId,
                            CancellationToken.None);
                    });
            }
            else
            {
                result = await service.AddFollowUpNoteAsync(
                    requestedLeadId,
                    request,
                    requestedAuthorId,
                    CancellationToken.None);
            }

            // Även en senare save ska vara säker efter avvisad input.
            await write.SaveChangesAsync();
        }

        var after = DateTimeOffset.UtcNow;

        await using var read = new AppIdentityDbContext(options);

        var notes = await read.FollowUpNotes
            .AsNoTracking()
            .ToListAsync();

        if (scenario is "valid" or "boundary")
        {
            var saved = Assert.Single(notes);

            Assert.NotNull(result);
            Assert.Equal(result.Id, saved.Id);
            Assert.Equal(lead.Id, saved.LeadId);
            Assert.Equal(author.Id, saved.UserId);
            Assert.Equal(text.Trim(), saved.Text);
            Assert.Equal(author.UserName, result.Author);
            Assert.Equal(saved.CreatedAt, result.CreatedAt);
            Assert.InRange(saved.CreatedAt, before, after);
        }
        else
        {
            Assert.Empty(notes);
            Assert.Null(result);
        }
    }
}