using System.ComponentModel.DataAnnotations;
using LeadFlow.Domain.Enums;

namespace LeadFlow.Application.DTOs.Leads;

public sealed class ChangeLeadStatusRequest
{
    [Required(ErrorMessage = "Status is required.")]
    public LeadStatus? Status { get; init; }

    public LeadOutcome? Outcome { get; init; }
}