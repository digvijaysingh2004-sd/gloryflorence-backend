using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class ExercisePrescriptionDetailConfiguration : IEntityTypeConfiguration<ExercisePrescriptionDetail>
    {
        public void Configure(EntityTypeBuilder<ExercisePrescriptionDetail> builder)
        {
            builder.ToTable("ExercisePrescriptionDetails");

            builder.HasKey(epd => epd.Id);

            builder.Property(epd => epd.Sets)
                .IsRequired();

            builder.Property(epd => epd.Repetitions)
                .IsRequired();

            builder.Property(epd => epd.HoldSeconds)
                .IsRequired();

            builder.Property(epd => epd.FrequencyPerDay)
                .IsRequired();

            builder.Property(epd => epd.DurationWeeks)
                .IsRequired();

            builder.Property(epd => epd.Instructions)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(epd => epd.Prescription)
                .WithMany(ep => ep.PrescriptionDetails)
                .HasForeignKey(epd => epd.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(epd => epd.Exercise)
                .WithMany(e => e.PrescriptionDetails)
                .HasForeignKey(epd => epd.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
