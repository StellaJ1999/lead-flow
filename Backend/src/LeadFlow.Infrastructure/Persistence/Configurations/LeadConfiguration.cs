using LeadFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.HasKey(x => x.Id);

        // Obligatoriska kolumner med tydliga längdgränser.
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(320);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(4000);

        // Company är nullable i domänen och förblir valfritt.
        builder.Property(x => x.Company).HasMaxLength(200);

        // Ett lead har många anteckningar.
        // FollowUpNote har ingen navigation tillbaka till Lead.
        builder.HasMany(x => x.FollowUpNotes)
            .WithOne()
            .HasForeignKey(x => x.LeadId)
            .OnDelete(DeleteBehavior.Cascade);

        // EF arbetar direkt mot den interna listan.
        builder.Navigation(x => x.FollowUpNotes)
            .HasField("_followUpNotes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}