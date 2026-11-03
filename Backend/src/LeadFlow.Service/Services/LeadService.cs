using LeadFlow.Application.Abstractions.Repositories;
using LeadFlow.Application.Abstractions.Services;
using LeadFlow.Application.DTOs.Leads;
using LeadFlow.Domain.Entities;

namespace LeadFlow.Service.Services;

public sealed class LeadService(ILeadRepository repository)
    : ILeadService
{
    public async Task<Guid> CreateAsync(
        CreateLeadRequest request,
        CancellationToken cancellationToken)
    {
        var lead = new Lead(
            request.Name,
            request.Email,
            request.Company,
            request.Message);

        await repository.AddAsync(lead, cancellationToken);

        return lead.Id;
    }

    public async Task<IReadOnlyList<LeadResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var leads = await repository.GetAllAsync(cancellationToken);

        return leads.Select(lead => new LeadResponse(
            lead.Id,
            lead.Name,
            lead.Email,
            lead.Company,
            lead.Status,
            lead.CreatedAt))
            .ToList();
    }

    public Task<LeadDetailsResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return repository.GetDetailsByIdAsync(
            id,
            cancellationToken);
    }

    public Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<LeadStatusResponse?> ChangeStatusAsync(
    Guid id,
    ChangeLeadStatusRequest request,
    CancellationToken cancellationToken)
    {
        if (request.Status is null)
            throw new ArgumentException("Status is required.");

        var lead = await repository.GetForUpdateAsync(
            id,
            cancellationToken);

        if (lead is null)
            return null;

        lead.ChangeStatus(
            request.Status.Value,
            request.Outcome);

        // Körs bara när domänreglerna accepterat ändringen.
        await repository.SaveChangesAsync(cancellationToken);

        return new LeadStatusResponse(
            lead.Id,
            lead.Status,
            lead.Outcome);
    }

    public async Task<FollowUpNoteResponse?> AddFollowUpNoteAsync(
    Guid leadId,
    CreateFollowUpNoteRequest request,
    string authorId,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(authorId))
            throw new UnauthorizedAccessException();

        var lead = await repository.GetForUpdateAsync(
            leadId,
            cancellationToken);

        if (lead is null)
            return null;

        var author = await repository.GetAuthorDisplayNameAsync(
            authorId,
            cancellationToken);

        if (author is null)
            throw new UnauthorizedAccessException();

        var note = lead.AddFollowUpNote(
            request.Text,
            authorId);

        repository.AddFollowUpNote(note);

        await repository.SaveChangesAsync(cancellationToken);

        return new FollowUpNoteResponse(
            note.Id,
            note.Text,
            author,
            note.CreatedAt);
    }
}