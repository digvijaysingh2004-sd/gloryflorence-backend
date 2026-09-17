using System;

namespace GloryFlorence.Application.DTOs
{
    public class PatientAssessmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int? SessionId { get; set; }
        public int PhysiotherapistId { get; set; }
        public string PhysiotherapistName { get; set; } = string.Empty;
        public DateTime AssessmentDate { get; set; }
        public string ChiefComplaint { get; set; } = string.Empty;
        public string CurrentCondition { get; set; } = string.Empty;
        public int PainLevel { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string ClinicalNotes { get; set; } = string.Empty;
        public string AssessmentNotes => ClinicalNotes;
        public string Recommendations { get; set; } = string.Empty;

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

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePatientAssessmentDto
    {
        public int PatientId { get; set; }
        public int? SessionId { get; set; }
        public int PhysiotherapistId { get; set; }
        public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
        public string ChiefComplaint { get; set; } = string.Empty;
        public string CurrentCondition { get; set; } = string.Empty;
        public int PainLevel { get; set; } // 0 - 10
        public string Diagnosis { get; set; } = string.Empty;
        public string ClinicalNotes { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;

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
    }

    public class UpdatePatientAssessmentDto
    {
        public int? SessionId { get; set; }
        public int PhysiotherapistId { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string ChiefComplaint { get; set; } = string.Empty;
        public string CurrentCondition { get; set; } = string.Empty;
        public int PainLevel { get; set; } // 0 - 10
        public string Diagnosis { get; set; } = string.Empty;
        public string ClinicalNotes { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;

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
    }

    public class PatientAssessmentFilterDto
    {
        public int? PatientId { get; set; }
        public int? PhysiotherapistId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }
}
