using LeadFlow.Infrastructure.Identity.Data;
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

        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}