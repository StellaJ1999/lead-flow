using System.ComponentModel.DataAnnotations;

namespace LeadFlow.Application.DTOs.Leads;

public sealed class CreateLeadRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [MaxLength(320)]
    public string Email { get; init; } = string.Empty;

    [MaxLength(200)]
    public string? Company { get; init; }

    [Required(ErrorMessage = "Message is required.")]
    [MaxLength(4000)]
    public string Message { get; init; } = string.Empty;
}