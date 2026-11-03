using LeadFlow.Application.DTOs.Leads;

namespace LeadFlow.Application.Abstractions.Services;

public interface ILeadService
{
    Task<Guid> CreateAsync(
        CreateLeadRequest request,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<LeadResponse>> GetAllAsync(
    CancellationToken cancellationToken);

    Task<LeadDetailsResponse?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken);

    Task<LeadStatusResponse?> ChangeStatusAsync(
    Guid id,
    ChangeLeadStatusRequest request,
    CancellationToken cancellationToken);

    Task<FollowUpNoteResponse?> AddFollowUpNoteAsync(
    Guid leadId,
    CreateFollowUpNoteRequest request,
    string authorId,
    CancellationToken cancellationToken);
}