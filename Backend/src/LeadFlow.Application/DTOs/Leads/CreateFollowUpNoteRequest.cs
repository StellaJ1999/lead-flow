using System.ComponentModel.DataAnnotations;

namespace LeadFlow.Application.DTOs.Leads;

public sealed class CreateFollowUpNoteRequest
{
    [Required(ErrorMessage = "Note text is required.")]
    [MaxLength(
        4000,
        ErrorMessage = "Note must not exceed 4000 characters.")]
    public string Text { get; init; } = string.Empty;
}