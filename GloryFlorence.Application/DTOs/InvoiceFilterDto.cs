namespace GloryFlorence.Application.DTOs
{
    public class InvoiceFilterDto
    {
        public int? PatientId { get; set; }
        public string? Status { get; set; }
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
