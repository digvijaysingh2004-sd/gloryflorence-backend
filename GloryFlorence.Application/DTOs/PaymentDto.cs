using System;

namespace GloryFlorence.Application.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
    }

    public class CreatePaymentDto
    {
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? TransactionReference { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Notes { get; set; }
    }
}
