using LeadFlow.Application.DTOs.Leads;

namespace LeadFlow.Application.Abstractions.Services;

public interface ILeadService
{
    Task<Guid> CreateAsync(
        CreateLeadRequest request,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<LeadResponse>> GetAllAsync(
    CancellationToken cancellationToken);
}