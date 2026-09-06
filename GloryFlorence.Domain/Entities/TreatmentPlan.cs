using System;
using System.Collections.Generic;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class TreatmentPlan : BaseEntity
    {
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;

        public int PhysiotherapistId { get; set; }
        public virtual User Physiotherapist { get; set; } = null!;

        public int AssessmentId { get; set; }
        public virtual PatientAssessment Assessment { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int NumberOfSessions { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft"; // Draft, Active, Completed, Discontinued

        // Navigation properties
        public virtual ICollection<TreatmentPlanDetail> TreatmentPlanDetails { get; set; } = new List<TreatmentPlanDetail>();
        public virtual ICollection<TreatmentSession> TreatmentSessions { get; set; } = new List<TreatmentSession>();
    }
}
