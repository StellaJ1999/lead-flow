using LeadFlow.Application.Abstractions.Services;
using LeadFlow.Application.DTOs.Leads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeadFlow.Api.Controllers;

[ApiController]
[Route("api/leads")]
[Authorize]
public sealed class LeadsController(ILeadService leadService)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLeadRequest request,
        CancellationToken cancellationToken)
    {
        var id = await leadService.CreateAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new { id });
    }
}