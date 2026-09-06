using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class Status : BaseEntity
    {
        public string Type { get; set; } = string.Empty; // Appointment, TreatmentPlan, Invoice, Payment
        public string Code { get; set; } = string.Empty; // e.g. SCHEDULED, CONFIRMED, etc.
        public string Name { get; set; } = string.Empty; // e.g. Scheduled, Confirmed, etc.
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
