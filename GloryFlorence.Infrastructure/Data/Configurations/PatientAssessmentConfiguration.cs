using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class PatientAssessmentConfiguration : IEntityTypeConfiguration<PatientAssessment>
    {
        public void Configure(EntityTypeBuilder<PatientAssessment> builder)
        {
            builder.ToTable("PatientAssessments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.AssessmentDate)
                .IsRequired();

            builder.Property(a => a.ChiefComplaint)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(a => a.CurrentCondition)
                .HasMaxLength(1000);

            builder.Property(a => a.PainLevel)
                .IsRequired();

            builder.Property(a => a.Diagnosis)
                .HasMaxLength(500);

            builder.Property(a => a.ClinicalNotes)
                .HasMaxLength(2000);

            builder.Property(a => a.Recommendations)
                .HasMaxLength(2000);

            // Relationships
            builder.HasOne(a => a.Patient)
                .WithMany(p => p.Assessments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Physiotherapist)
                .WithMany()
                .HasForeignKey(a => a.PhysiotherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Session)
                .WithMany(s => s.PatientAssessments)
                .HasForeignKey(a => a.SessionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
