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

    [HttpGet]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var leads = await leadService.GetAllAsync(cancellationToken);

        return Ok(leads);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLeadRequest request,
        CancellationToken cancellationToken)
    {
        await leadService.CreateAsync(request, cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                message = "Your interest has been registered."
            });
    }


}