namespace GloryFlorence.Application.DTOs
{
    public class CreatePatientDocumentDto
    {
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}
