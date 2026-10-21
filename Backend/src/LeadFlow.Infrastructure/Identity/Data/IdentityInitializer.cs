
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeadFlow.Infrastructure.Identity.Data;

public static class IdentityInitializer
{
    public static async Task SeedAsync(
    IServiceProvider services,
    IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var email = configuration["Seed:StaffEmail"]?.Trim();
        var password = configuration["Seed:StaffPassword"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Staff seed email and password must be configured.");
        }

        //Repeterade startups får inte duplicera konton
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
            return;

        var staff = new IdentityUser
        {
            UserName = email,
            Email = email,

            EmailConfirmed = true
        };

        //UserManager validerar och hashar lösen
        var result = await userManager.CreateAsync(staff, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Could not seed staff account: {errors}");
        }
    }
}
