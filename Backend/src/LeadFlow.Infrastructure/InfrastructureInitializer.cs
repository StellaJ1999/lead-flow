using LeadFlow.Application.Abstractions.Security;
using LeadFlow.Infrastructure.Identity.Data;
using LeadFlow.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeadFlow.Infrastructure;

public static class InfrastructureInitializer
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "Connection string 'SqlServer' is missing.");

        // Registrerar kontexten och väljer SQL Server som databasleverantör.
        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Identity använder samma kontext för användare och roller.
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();
        
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}