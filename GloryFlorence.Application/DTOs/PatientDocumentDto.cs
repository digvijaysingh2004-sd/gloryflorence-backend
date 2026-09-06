using System;

namespace GloryFlorence.Application.DTOs
{
    public class PatientDocumentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
