using LeadFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeadFlow.Infrastructure.Identity.Data;

public sealed class AppIdentityDbContext
    : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public AppIdentityDbContext(
        DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }

    // Gör entiteterna tillgängliga för frågor och sparande via EF Core.
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<FollowUpNote> FollowUpNotes => Set<FollowUpNote>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Registrerar Identity-tabeller och deras relationer.
        base.OnModelCreating(builder);

        // Letar efter IEntityTypeConfiguration<T> i Infrastructure.
        builder.ApplyConfigurationsFromAssembly(
            typeof(AppIdentityDbContext).Assembly);
    }
}