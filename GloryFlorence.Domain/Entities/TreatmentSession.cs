using System;
using System.Collections.Generic;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class TreatmentSession : BaseEntity
    {
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; } = null!;

        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;

        public int PhysiotherapistId { get; set; }
        public virtual User Physiotherapist { get; set; } = null!;

        public int? TreatmentPlanId { get; set; }
        public virtual TreatmentPlan? TreatmentPlan { get; set; }

        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int? PainLevelBefore { get; set; } // Scale 0-10
        public int? PainLevelAfter { get; set; }  // Scale 0-10

        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled, NoShow

        public string Assessment { get; set; } = string.Empty;
        public string TreatmentPerformed { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string? ModalitiesConducted { get; set; }
        public string? PatientTolerance { get; set; }
        public string? NextSessionPlan { get; set; }

        // Navigation properties
        public virtual ICollection<PatientAssessment> PatientAssessments { get; set; } = new List<PatientAssessment>();
    }
}
