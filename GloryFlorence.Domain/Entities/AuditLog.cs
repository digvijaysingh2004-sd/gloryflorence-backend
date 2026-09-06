using System;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        public string Action { get; set; } = string.Empty; // e.g. CREATE, UPDATE, DELETE, STATUS_CHANGE
        public string EntityName { get; set; } = string.Empty; // e.g. PatientAssessment, TreatmentPlan, TreatmentSession
        public string EntityId { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IPAddress { get; set; }
    }
}
