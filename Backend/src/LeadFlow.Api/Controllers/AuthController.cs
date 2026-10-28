using LeadFlow.Application.Abstractions.Security;
using LeadFlow.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;

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

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email)
        });
    }

    [AllowAnonymous]
    [HttpGet("csrf")]

    //Svaret cachelagras inte.
    [ResponseCache(
    NoStore = true,
    Location = ResponseCacheLocation.None)]
    public IActionResult GetCsrfToken(
    [FromServices] IAntiforgery antiforgery)
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        //Skapar tokeninformationen och ser till att antiforgery-cookien finns i svaret när det behövs.
        return Ok(new
        {
            token = tokens.RequestToken
        });
    }
}