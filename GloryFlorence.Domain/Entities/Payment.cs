using System;

namespace GloryFlorence.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;

        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Credit Card, Bank Transfer, Insurance
        public string? TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}
