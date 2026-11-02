using LeadFlow.Domain.Enums;

namespace LeadFlow.Application.DTOs.Leads;

public sealed record LeadStatusResponse(
    Guid Id,
    LeadStatus Status,
    LeadOutcome? Outcome);