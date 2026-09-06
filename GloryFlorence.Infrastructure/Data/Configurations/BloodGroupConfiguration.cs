using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class BloodGroupConfiguration : IEntityTypeConfiguration<BloodGroup>
    {
        public void Configure(EntityTypeBuilder<BloodGroup> builder)
        {
            builder.ToTable("BloodGroups");
            builder.HasKey(bg => bg.Id);
            builder.Property(bg => bg.Name).IsRequired().HasMaxLength(10);
        }
    }
}
