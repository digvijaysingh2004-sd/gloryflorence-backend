using System;
using System.Collections.Generic;

namespace GloryFlorence.Application.DTOs
{
    public class CreateInvoiceItemDto
    {
        public int? TreatmentTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CreateInvoiceDto
    {
        public int PatientId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }
}
