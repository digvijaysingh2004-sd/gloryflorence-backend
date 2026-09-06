using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class PatientMedicalHistoryConfiguration : IEntityTypeConfiguration<PatientMedicalHistory>
    {
        public void Configure(EntityTypeBuilder<PatientMedicalHistory> builder)
        {
            builder.ToTable("PatientMedicalHistory");
            builder.HasKey(pmh => pmh.Id);
            builder.Property(pmh => pmh.Diagnosis).IsRequired().HasMaxLength(250);
            builder.Property(pmh => pmh.Symptoms).HasMaxLength(500);
            builder.Property(pmh => pmh.TreatmentReceived).HasMaxLength(500);
            builder.Property(pmh => pmh.Remarks).HasMaxLength(500);

            builder.HasOne(pmh => pmh.Patient)
                .WithMany(p => p.MedicalHistories)
                .HasForeignKey(pmh => pmh.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
