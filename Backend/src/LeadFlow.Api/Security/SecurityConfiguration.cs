using Microsoft.AspNetCore.Mvc;

namespace LeadFlow.Api.Security;

public static class SecurityConfiguration
{
    public static IServiceCollection AddSecurityConfiguration(
        this IServiceCollection services)
    {
        // Validerar CSRF på POST, PUT, PATCH och DELETE.
        // Omfattar även login, logout och publik intresseanmälan.
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(
                new AutoValidateAntiforgeryTokenAttribute());
        });

        //AddAntiforgery registrerar tjänsterna som skapar och validerar antiforgery-token.
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";

            //Prefixet __Host- kräver HTTPS
            options.Cookie.Name = "__Host-LeadFlow.Antiforgery";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.Path = "/";
        });

        // Identity är redan registrerat i AddInfrastructure.
        // Här konfigurerar vi dess befintliga autentiseringscookie.
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "__Host-LeadFlow.Auth";

            //Js kan inte läsa cookien
            options.Cookie.HttpOnly = true;

            //Skickas över https
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            //Begränsar när browsern skickar cookien i samband med trafik från andra webbplatser
            options.Cookie.SameSite = SameSiteMode.Strict;

            //Gör att cookien kan skickas till alla paths på värden, inklusive /api/auth/me och /api/leads
            options.Cookie.Path = "/";

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                return Task.CompletedTask;
            };
        });

        return services;
    }
}