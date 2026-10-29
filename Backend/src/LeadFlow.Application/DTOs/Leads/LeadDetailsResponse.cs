using LeadFlow.Domain.Enums;

namespace LeadFlow.Application.DTOs.Leads;

public sealed record LeadDetailsResponse(
    Guid Id,
    string Name,
    string Email,
    string? Company,
    string Message,
    LeadStatus Status,
    DateTimeOffset CreatedAt);