

using LeadFlow.Application.Abstractions.Security;
using LeadFlow.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace LeadFlow.Infrastructure.Identity.Services;

public sealed class AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : IAuthService
{
    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null)
            return false;

        var result = await signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        return result.Succeeded;
    }
}
