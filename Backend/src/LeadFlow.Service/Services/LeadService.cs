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
}