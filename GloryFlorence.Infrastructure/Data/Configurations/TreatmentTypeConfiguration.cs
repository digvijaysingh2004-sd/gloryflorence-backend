using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class TreatmentTypeConfiguration : IEntityTypeConfiguration<TreatmentType>
    {
        public void Configure(EntityTypeBuilder<TreatmentType> builder)
        {
            builder.ToTable("TreatmentTypes");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.DefaultPrice)
                .HasPrecision(18, 2);

            builder.Property(t => t.DefaultDurationMinutes)
                .HasDefaultValue(30);

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true);

            builder.HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
