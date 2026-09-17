using System;
using System.Collections.Generic;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class PatientAssessment : BaseEntity
    {
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;

        public int? SessionId { get; set; }
        public virtual TreatmentSession? Session { get; set; }

        public int PhysiotherapistId { get; set; }
        public virtual User Physiotherapist { get; set; } = null!;

        public DateTime AssessmentDate { get; set; }
        public string ChiefComplaint { get; set; } = string.Empty;
        public string CurrentCondition { get; set; } = string.Empty;
        public int PainLevel { get; set; } // Scale 0 to 10
        public string Diagnosis { get; set; } = string.Empty;
        public string ClinicalNotes { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;

        // Additional clinical assessment fields
        public string? PainLocation { get; set; }
        public string? PainType { get; set; }
        public string? AggravatingFactors { get; set; }
        public string? RelievingFactors { get; set; }
        public string? RomFindings { get; set; }
        public string? PostureAndGait { get; set; }
        public string? FunctionalLimitations { get; set; }
        public string? Prognosis { get; set; }
        public string? ShortTermGoals { get; set; }
        public string? LongTermGoals { get; set; }
        public string? RecommendedFrequency { get; set; }

        // Navigation properties
        public virtual ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
    }
}
