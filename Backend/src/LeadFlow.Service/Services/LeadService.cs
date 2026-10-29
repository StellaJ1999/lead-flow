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

    public async Task<LeadDetailsResponse?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var lead = await repository.GetByIdAsync(
            id,
            cancellationToken);

        if (lead is null)
        {
            return null;
        }

        return new LeadDetailsResponse(
            lead.Id,
            lead.Name,
            lead.Email,
            lead.Company,
            lead.Message,
            lead.Status,
            lead.CreatedAt);
    }
}