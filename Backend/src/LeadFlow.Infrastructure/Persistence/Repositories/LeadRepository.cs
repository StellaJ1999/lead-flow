using LeadFlow.Application.Abstractions.Repositories;
using LeadFlow.Domain.Entities;
using LeadFlow.Infrastructure.Identity.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IReadOnlyList<Lead>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Leads
            .AsNoTracking()
            .OrderByDescending(lead => lead.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}