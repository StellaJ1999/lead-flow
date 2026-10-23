using LeadFlow.Application.Abstractions.Repositories;
using LeadFlow.Domain.Entities;
using LeadFlow.Infrastructure.Identity.Data;

namespace LeadFlow.Infrastructure.Persistence.Repositories;

public sealed class LeadRepository(AppIdentityDbContext context)
    : ILeadRepository
{
    public async Task AddAsync(
        Lead lead,
        CancellationToken cancellationToken)
    {
        context.Leads.Add(lead);

        await context.SaveChangesAsync(cancellationToken);
    }
}