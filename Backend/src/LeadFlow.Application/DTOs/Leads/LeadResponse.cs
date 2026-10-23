using LeadFlow.Domain.Enums;

namespace LeadFlow.Application.DTOs.Leads;

public sealed record LeadResponse(
    Guid Id,
    string Name,
    string Email,
    string? Company,
    LeadStatus Status,
    DateTimeOffset CreatedAt);