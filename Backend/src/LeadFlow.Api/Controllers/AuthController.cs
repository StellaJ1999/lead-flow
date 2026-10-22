using LeadFlow.Application.Abstractions.Security;
using LeadFlow.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeadFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService)
    : ControllerBase
{

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var succeeded = await authService.LoginAsync(request);

        if (!succeeded)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return NoContent();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout() 
    {
        await authService.LogoutAsync();
        return NoContent();
    }
}