using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class TreatmentSessionConfiguration : IEntityTypeConfiguration<TreatmentSession>
    {
        public void Configure(EntityTypeBuilder<TreatmentSession> builder)
        {
            builder.ToTable("TreatmentSessions");

            builder.HasKey(ts => ts.Id);

            builder.Property(ts => ts.SessionDate)
                .IsRequired();

            builder.Property(ts => ts.StartTime)
                .IsRequired();

            builder.Property(ts => ts.EndTime)
                .IsRequired();

            builder.Property(ts => ts.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Scheduled");

            builder.Property(ts => ts.Assessment)
                .HasMaxLength(1000);

            builder.Property(ts => ts.TreatmentPerformed)
                .HasMaxLength(1000);

            builder.Property(ts => ts.Recommendations)
                .HasMaxLength(1000);

            builder.Property(ts => ts.Notes)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(ts => ts.Appointment)
                .WithMany(a => a.TreatmentSessions)
                .HasForeignKey(ts => ts.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ts => ts.Patient)
                .WithMany(p => p.TreatmentSessions)
                .HasForeignKey(ts => ts.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ts => ts.Physiotherapist)
                .WithMany()
                .HasForeignKey(ts => ts.PhysiotherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ts => ts.TreatmentPlan)
                .WithMany(tp => tp.TreatmentSessions)
                .HasForeignKey(ts => ts.TreatmentPlanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
