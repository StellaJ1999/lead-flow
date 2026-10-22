using LeadFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FollowUpNoteConfiguration
    : IEntityTypeConfiguration<FollowUpNote>
{
    public void Configure(EntityTypeBuilder<FollowUpNote> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);

        // Databasen kräver att författarens ID finns bland Identity-användarna.
        // Identity-referensen stannar i Infrastructure.
        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}