using System;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class PatientDocument : BaseEntity
    {
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
