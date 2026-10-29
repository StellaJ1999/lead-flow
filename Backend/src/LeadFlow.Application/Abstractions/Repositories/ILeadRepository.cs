using LeadFlow.Domain.Entities;

namespace LeadFlow.Application.Abstractions.Repositories;

public interface ILeadRepository
{
    Task AddAsync(
        Lead lead,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Lead>> GetAllAsync(
    CancellationToken cancellationToken);

    Task<Lead?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken);
}