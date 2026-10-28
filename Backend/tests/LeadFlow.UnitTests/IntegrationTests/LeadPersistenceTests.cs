using LeadFlow.Application.DTOs.Leads;
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
}