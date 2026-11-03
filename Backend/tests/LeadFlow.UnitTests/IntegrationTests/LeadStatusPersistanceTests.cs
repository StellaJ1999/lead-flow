using LeadFlow.Application.DTOs.Leads;
using LeadFlow.Domain.Entities;
using LeadFlow.Domain.Enums;
using LeadFlow.Infrastructure.Identity.Data;
using LeadFlow.Infrastructure.Persistence.Repositories;
using LeadFlow.Service.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LeadFlow.UnitTests.IntegrationTests;

public class LeadStatusPersistenceTests
{
    // Kontrollerar att databasen bara sparar tillåtna statusändringar.
    [Theory]
    // initial status, requested status, outcome, accepted
    [InlineData(0, 1, null, true)]
    [InlineData(0, 2, 0, true)]
    [InlineData(1, 2, 1, true)]
    [InlineData(2, 1, null, true)]
    [InlineData(1, 1, null, true)]
    [InlineData(2, 2, 0, true)]
    [InlineData(0, 2, null, false)]
    [InlineData(0, 2, 999, false)]
    [InlineData(0, 1, 0, false)]
    [InlineData(1, 0, null, false)]
    [InlineData(2, 0, null, false)]
    [InlineData(2, 2, 1, false)]
    [InlineData(0, 999, null, false)]
    public async Task ChangeStatusAsync_PersistsOnlyAcceptedChanges(
        int initialStatus,
        int requestedStatus,
        int? requestedOutcome,
        bool accepted)
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppIdentityDbContext>()
            .UseSqlite(connection)
            .Options;

        var lead = new Lead(
            "Status test",
            "status@example.com",
            null,
            "Test lead status changes.");

        if (initialStatus == 1)
            lead.MarkAsContacted();
        else if (initialStatus == 2)
            lead.Close(LeadOutcome.Customer);

        var originalStatus = lead.Status;
        var originalOutcome = lead.Outcome;

        await using (var setup = new AppIdentityDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.Leads.Add(lead);
            await setup.SaveChangesAsync();
        }

        var request = new ChangeLeadStatusRequest
        {
            Status = (LeadStatus)requestedStatus,
            Outcome = requestedOutcome.HasValue
                ? (LeadOutcome)requestedOutcome.Value
                : null
        };

        await using (var write = new AppIdentityDbContext(options))
        {
            var service = new LeadService(new LeadRepository(write));

            if (accepted)
            {
                var result = await service.ChangeStatusAsync(
                    lead.Id,
                    request,
                    CancellationToken.None);

                Assert.NotNull(result);
                Assert.Equal(request.Status!.Value, result.Status);
                Assert.Equal(request.Outcome, result.Outcome);
            }
            else
            {
                var exception = await Record.ExceptionAsync(async () =>
                {
                    await service.ChangeStatusAsync(
                        lead.Id,
                        request,
                        CancellationToken.None);
                });

                Assert.True(
                    exception is ArgumentException
                        or InvalidOperationException);

                // Även entiteten i minnet ska vara oförändrad.
                var trackedLead = write.ChangeTracker
                    .Entries<Lead>()
                    .Single()
                    .Entity;

                Assert.Equal(originalStatus, trackedLead.Status);
                Assert.Equal(originalOutcome, trackedLead.Outcome);

                // Verifierar att en senare save inte läcker ändringar.
                await write.SaveChangesAsync();
            }
        }

        await using var read = new AppIdentityDbContext(options);

        var saved = await read.Leads
            .AsNoTracking()
            .SingleAsync(item => item.Id == lead.Id);

        Assert.Equal(
            accepted ? request.Status!.Value : originalStatus,
            saved.Status);

        Assert.Equal(
            accepted ? request.Outcome : originalOutcome,
            saved.Outcome);
    }
}