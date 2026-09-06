using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.ToTable("Statuses");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .HasMaxLength(250);

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(s => new { s.Type, s.Code })
                .IsUnique();
        }
    }
}
