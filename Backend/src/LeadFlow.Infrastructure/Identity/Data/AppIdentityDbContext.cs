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

    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<FollowUpNote> FollowUpNotes => Set<FollowUpNote>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(AppIdentityDbContext).Assembly);
    }
}