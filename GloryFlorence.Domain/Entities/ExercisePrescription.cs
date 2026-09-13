using System;
using System.Collections.Generic;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class ExercisePrescription : BaseEntity
    {
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;

        public int PhysiotherapistId { get; set; }
        public virtual User Physiotherapist { get; set; } = null!;

        public int TreatmentPlanId { get; set; }
        public virtual TreatmentPlan TreatmentPlan { get; set; } = null!;

        public DateTime PrescriptionDate { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public string Status { get; set; } = "Active"; // Active, Completed, Discontinued
        public string? Diagnosis { get; set; }
        public string? TargetGoal { get; set; }

        // Navigation property for prescribed exercises
        public virtual ICollection<ExercisePrescriptionDetail> PrescriptionDetails { get; set; } = new List<ExercisePrescriptionDetail>();
    }
}
