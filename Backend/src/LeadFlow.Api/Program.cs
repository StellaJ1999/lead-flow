using LeadFlow.Application.Abstractions.Services;
using LeadFlow.Infrastructure;
using LeadFlow.Infrastructure.Identity.Data;
using LeadFlow.Service.Services;
using LeadFlow.Api.Security;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSecurityConfiguration();

builder.Services.AddScoped<ILeadService, LeadService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await IdentityInitializer.SeedAsync(
        app.Services,
        app.Configuration);
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


