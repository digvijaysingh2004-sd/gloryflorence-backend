using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class ExercisePrescriptionConfiguration : IEntityTypeConfiguration<ExercisePrescription>
    {
        public void Configure(EntityTypeBuilder<ExercisePrescription> builder)
        {
            builder.ToTable("ExercisePrescriptions");

            builder.HasKey(ep => ep.Id);

            builder.Property(ep => ep.PrescriptionDate)
                .IsRequired();

            builder.Property(ep => ep.Instructions)
                .HasMaxLength(2000);

            builder.Property(ep => ep.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            // Relationships
            builder.HasOne(ep => ep.Patient)
                .WithMany(p => p.ExercisePrescriptions)
                .HasForeignKey(ep => ep.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ep => ep.Physiotherapist)
                .WithMany()
                .HasForeignKey(ep => ep.PhysiotherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ep => ep.TreatmentPlan)
                .WithMany(tp => tp.ExercisePrescriptions)
                .HasForeignKey(ep => ep.TreatmentPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
