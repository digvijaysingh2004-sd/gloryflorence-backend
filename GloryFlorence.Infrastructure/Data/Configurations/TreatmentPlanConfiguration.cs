using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class TreatmentPlanConfiguration : IEntityTypeConfiguration<TreatmentPlan>
    {
        public void Configure(EntityTypeBuilder<TreatmentPlan> builder)
        {
            builder.ToTable("TreatmentPlans");

            builder.HasKey(tp => tp.Id);

            builder.Property(tp => tp.StartDate)
                .IsRequired();

            builder.Property(tp => tp.ExpectedEndDate)
                .IsRequired();

            builder.Property(tp => tp.NumberOfSessions)
                .IsRequired();

            builder.Property(tp => tp.Goal)
                .HasMaxLength(1000);

            builder.Property(tp => tp.Notes)
                .HasMaxLength(2000);

            builder.Property(tp => tp.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Draft");

            // Relationships
            builder.HasOne(tp => tp.Patient)
                .WithMany(p => p.TreatmentPlans)
                .HasForeignKey(tp => tp.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tp => tp.Physiotherapist)
                .WithMany()
                .HasForeignKey(tp => tp.PhysiotherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tp => tp.Assessment)
                .WithMany(a => a.TreatmentPlans)
                .HasForeignKey(tp => tp.AssessmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
