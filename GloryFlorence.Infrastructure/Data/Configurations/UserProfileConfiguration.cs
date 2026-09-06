using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");

            builder.HasKey(up => up.Id);

            builder.Property(up => up.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(up => up.Gender)
                .HasMaxLength(15);

            builder.Property(up => up.Address)
                .HasMaxLength(250);

            builder.Property(up => up.Bio)
                .HasMaxLength(500);

            builder.Property(up => up.ProfilePictureUrl)
                .HasMaxLength(500);

            builder.HasOne(up => up.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
