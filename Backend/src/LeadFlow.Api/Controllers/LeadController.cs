using LeadFlow.Application.Abstractions.Services;
using LeadFlow.Application.DTOs.Leads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [HttpGet("{id:guid}")]
    [ResponseCache(
    NoStore = true,
    Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var lead = await leadService.GetByIdAsync(
            id,
            cancellationToken);

        if (lead is null)
        {
            return NotFound(new
            {
                message = "Lead not found."
            });
        }

        return Ok(lead);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken cancellationToken)
    {
        var deleted = await leadService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Lead not found."
            });
        }

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
    Guid id,
    [FromBody] ChangeLeadStatusRequest request,
    CancellationToken cancellationToken)
    {
        LeadStatusResponse? result;

        try
        {
            result = await leadService.ChangeStatusAsync(
                id,
                request,
                cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }

        if (result is null)
        {
            return NotFound(new
            {
                message = "Lead not found."
            });
        }

        return Ok(result);
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddFollowUpNote(
    Guid id,
    [FromBody] CreateFollowUpNoteRequest request,
    CancellationToken cancellationToken)
    {
        var authorId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(authorId))
            return Unauthorized();

        FollowUpNoteResponse? note;

        try
        {
            note = await leadService.AddFollowUpNoteAsync(
                id,
                request,
                authorId,
                cancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException)
        {
            // Reservskydd om domänvalideringen avvisar texten.
            ModelState.AddModelError(
                nameof(CreateFollowUpNoteRequest.Text),
                "Note text is required and must not exceed 4000 characters.");

            return ValidationProblem(ModelState);
        }

        if (note is null)
        {
            return NotFound(new
            {
                message = "Lead not found."
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            note);
    }
}