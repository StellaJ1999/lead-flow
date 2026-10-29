using LeadFlow.Application.DTOs.Leads;
using LeadFlow.Domain.Entities;
using LeadFlow.Domain.Enums;
using LeadFlow.Infrastructure.Identity.Data;
using LeadFlow.Infrastructure.Persistence.Repositories;
using LeadFlow.Service.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LeadFlow.UnitTests.IntegrationTests;

public class LeadPersistenceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("Example AB")]
    public async Task CreateAsync_PersistsLead_ReadableFromNewContext(
        string? company)
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppIdentityDbContext>()
            .UseSqlite(connection)
            .Options;

        var request = new CreateLeadRequest
        {
            Name = "Anna Andersson",
            Email = "anna@example.com",
            Company = company,
            Message = "Would like more information."
        };

        var before = DateTimeOffset.UtcNow;
        Guid leadId;

        await using (var writeContext = new AppIdentityDbContext(options))
        {
            await writeContext.Database.EnsureCreatedAsync();

            var repository = new LeadRepository(writeContext);
            var service = new LeadService(repository);

            leadId = await service.CreateAsync(
                request,
                CancellationToken.None);
        }

        var after = DateTimeOffset.UtcNow;

        // En ny context säkerställer att värdena hämtas från databasen.
        await using var readContext = new AppIdentityDbContext(options);

        var savedLead = await readContext.Leads
            .AsNoTracking()
            .SingleAsync(lead => lead.Id == leadId);

        Assert.NotEqual(Guid.Empty, leadId);
        Assert.Equal(request.Name, savedLead.Name);
        Assert.Equal(request.Email, savedLead.Email);
        Assert.Equal(company, savedLead.Company);
        Assert.Equal(request.Message, savedLead.Message);
        Assert.Equal(LeadStatus.New, savedLead.Status);
        Assert.Null(savedLead.Outcome);
        Assert.InRange(savedLead.CreatedAt, before, after);
    }

    [Fact]
    public async Task DeleteAsync_RemovesOnlySelectedLead()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppIdentityDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new AppIdentityDbContext(options);

        await context.Database.EnsureCreatedAsync();

        var selected = new Lead(
            "Delete test",
            "delete@example.com",
            null,
            "This lead should be deleted.");

        var remaining = new Lead(
            "Keep test",
            "keep@example.com",
            null,
            "This lead should remain.");

        context.Leads.AddRange(selected, remaining);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var repository = new LeadRepository(context);

        var deleted = await repository.DeleteAsync(
            selected.Id,
            CancellationToken.None);

        Assert.True(deleted);

        Assert.False(await context.Leads.AnyAsync(
            lead => lead.Id == selected.Id));

        Assert.True(await context.Leads.AnyAsync(
            lead => lead.Id == remaining.Id));

        var deletedAgain = await repository.DeleteAsync(
            selected.Id,
            CancellationToken.None);

        Assert.False(deletedAgain);
    }
}