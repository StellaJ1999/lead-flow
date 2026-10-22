using LeadFlow.Infrastructure;
using LeadFlow.Infrastructure.Identity.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await IdentityInitializer.SeedAsync(
        app.Services,
        app.Configuration);
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


