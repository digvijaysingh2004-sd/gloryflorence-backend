using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.EntityId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.OldValue)
                .HasMaxLength(4000);

            builder.Property(al => al.NewValue)
                .HasMaxLength(4000);

            builder.Property(al => al.IPAddress)
                .HasMaxLength(50);

            // Relationships
            builder.HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(al => al.EntityName);
            builder.HasIndex(al => al.CreatedAt);
        }
    }
}
