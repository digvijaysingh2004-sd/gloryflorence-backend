using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class TreatmentPlanDetailConfiguration : IEntityTypeConfiguration<TreatmentPlanDetail>
    {
        public void Configure(EntityTypeBuilder<TreatmentPlanDetail> builder)
        {
            builder.ToTable("TreatmentPlanDetails");

            builder.HasKey(tpd => tpd.Id);

            builder.Property(tpd => tpd.Frequency)
                .HasMaxLength(100);

            builder.Property(tpd => tpd.DurationMinutes)
                .IsRequired();

            builder.Property(tpd => tpd.NumberOfSessions)
                .IsRequired();

            builder.Property(tpd => tpd.Instructions)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(tpd => tpd.TreatmentPlan)
                .WithMany(tp => tp.TreatmentPlanDetails)
                .HasForeignKey(tpd => tpd.TreatmentPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tpd => tpd.TreatmentType)
                .WithMany()
                .HasForeignKey(tpd => tpd.TreatmentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
