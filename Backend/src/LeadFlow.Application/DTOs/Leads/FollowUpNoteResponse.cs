namespace LeadFlow.Application.DTOs.Leads;

public sealed record FollowUpNoteResponse(
    Guid Id,
    string Text,
    string Author,
    DateTimeOffset CreatedAt);