using LeadFlow.Domain.Entities;

namespace LeadFlow.Application.Abstractions.Repositories;

public interface ILeadRepository
{
    Task AddAsync(
        Lead lead,
        CancellationToken cancellationToken);
}