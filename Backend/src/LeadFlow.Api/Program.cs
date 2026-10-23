using LeadFlow.Application.Abstractions.Services;
using LeadFlow.Infrastructure;
using LeadFlow.Infrastructure.Identity.Data;
using LeadFlow.Service.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ILeadService, LeadService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await IdentityInitializer.SeedAsync(
        app.Services,
        app.Configuration);
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


