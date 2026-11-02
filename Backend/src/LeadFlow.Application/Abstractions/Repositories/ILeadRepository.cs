using LeadFlow.Domain.Entities;
using LeadFlow.Application.DTOs.Leads;

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

    Task<LeadDetailsResponse?> GetDetailsByIdAsync(
    Guid id,
    CancellationToken cancellationToken);

    Task<Lead?> GetForUpdateAsync(
    Guid id,
    CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}