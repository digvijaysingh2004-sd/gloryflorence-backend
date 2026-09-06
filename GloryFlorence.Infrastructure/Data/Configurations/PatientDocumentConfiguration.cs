using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloryFlorence.Infrastructure.Data.Configurations
{
    public class PatientDocumentConfiguration : IEntityTypeConfiguration<PatientDocument>
    {
        public void Configure(EntityTypeBuilder<PatientDocument> builder)
        {
            builder.ToTable("PatientDocuments");
            builder.HasKey(pd => pd.Id);
            builder.Property(pd => pd.DocumentName).IsRequired().HasMaxLength(250);
            builder.Property(pd => pd.DocumentType).IsRequired().HasMaxLength(100);
            builder.Property(pd => pd.FilePath).IsRequired().HasMaxLength(500);

            builder.HasOne(pd => pd.Patient)
                .WithMany(p => p.Documents)
                .HasForeignKey(pd => pd.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
