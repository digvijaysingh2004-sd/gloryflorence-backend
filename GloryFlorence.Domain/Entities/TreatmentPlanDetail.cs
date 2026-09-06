using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class TreatmentPlanDetail : BaseEntity
    {
        public int TreatmentPlanId { get; set; }
        public virtual TreatmentPlan TreatmentPlan { get; set; } = null!;

        public int TreatmentTypeId { get; set; }
        public virtual TreatmentType TreatmentType { get; set; } = null!;

        public string Frequency { get; set; } = string.Empty; // e.g. "3x per week", "Daily"
        public int DurationMinutes { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public int NumberOfSessions { get; set; }
    }
}
