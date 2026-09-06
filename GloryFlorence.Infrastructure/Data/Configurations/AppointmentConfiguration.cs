using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.AppointmentDate)
                .IsRequired();

            builder.Property(a => a.StartTime)
                .IsRequired();

            builder.Property(a => a.EndTime)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Scheduled");

            builder.Property(a => a.Reason)
                .HasMaxLength(250);

            builder.Property(a => a.Notes)
                .HasMaxLength(1000);

            builder.Property(a => a.CancellationReason)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Physiotherapist)
                .WithMany()
                .HasForeignKey(a => a.PhysiotherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AppointmentType)
                .WithMany()
                .HasForeignKey(a => a.AppointmentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Therapist)
                .WithMany(t => t.Appointments)
                .HasForeignKey(a => a.TherapistId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
