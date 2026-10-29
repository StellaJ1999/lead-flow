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

    public async Task<Lead?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await context.Leads
            .AsNoTracking()
            .SingleOrDefaultAsync(
                lead => lead.Id == id,
                cancellationToken);
    }

    public async Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var deletedRows = await context.Leads
            .Where(lead => lead.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedRows > 0;
    }
}