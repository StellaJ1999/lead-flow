using System.ComponentModel.DataAnnotations;

namespace LeadFlow.Application.DTOs.Leads;

public sealed class CreateLeadRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name must not exceed 200 characters.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [MaxLength(320, ErrorMessage = "Email must not exceed 320 characters.")]
    public string Email { get; init; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Company name must not exceed 200 characters.")]
    public string? Company { get; init; }

    [Required(ErrorMessage = "Message is required.")]
    [MaxLength(4000, ErrorMessage = "Message must not exceed 4000 characters.")]
    public string Message { get; init; } = string.Empty;
}