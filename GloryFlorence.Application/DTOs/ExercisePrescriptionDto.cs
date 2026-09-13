using System;
using System.Collections.Generic;

namespace GloryFlorence.Application.DTOs
{
    public class ExercisePrescriptionDetailDto
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public string ExerciseTitle => ExerciseName;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Category => CategoryName;
        public string TargetMuscleGroup { get; set; } = string.Empty;
        public string ExerciseDescription { get; set; } = string.Empty;
        public string ExerciseInstructions { get; set; } = string.Empty;
        public string ExerciseVideoUrl { get; set; } = string.Empty;
        public string ExerciseImageUrl { get; set; } = string.Empty;
        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public int Reps => Repetitions;
        public int HoldSeconds { get; set; }
        public int HoldSec => HoldSeconds;
        public int FrequencyPerDay { get; set; }
        public string Frequency => $"{FrequencyPerDay}x / day";
        public int DurationWeeks { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public string Notes => Instructions;
    }

    public class CreateExercisePrescriptionDetailDto
    {
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public int HoldSeconds { get; set; }
        public int FrequencyPerDay { get; set; }
        public int DurationWeeks { get; set; }
        public string Instructions { get; set; } = string.Empty;
    }

    public class ExercisePrescriptionDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PhysiotherapistId { get; set; }
        public string PhysiotherapistName { get; set; } = string.Empty;
        public string PrescribedBy => PhysiotherapistName;
        public int TreatmentPlanId { get; set; }
        public string TreatmentPlanGoal { get; set; } = string.Empty;
        public string? TargetGoal { get; set; }
        public string? Diagnosis { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public DateTime PrescribedDate => PrescriptionDate;
        public string Instructions { get; set; } = string.Empty;
        public string GeneralInstructions => Instructions;
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ExercisePrescriptionDetailDto> PrescriptionDetails { get; set; } = new List<ExercisePrescriptionDetailDto>();
        public List<ExercisePrescriptionDetailDto> Items => PrescriptionDetails;
    }

    public class CreateExercisePrescriptionDto
    {
        public int PatientId { get; set; }
        public int PhysiotherapistId { get; set; }
        public int TreatmentPlanId { get; set; }
        public DateTime PrescriptionDate { get; set; } = DateTime.UtcNow;
        public string Instructions { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public string? Diagnosis { get; set; }
        public string? TargetGoal { get; set; }
        public List<CreateExercisePrescriptionDetailDto> PrescriptionDetails { get; set; } = new List<CreateExercisePrescriptionDetailDto>();
    }

    public class UpdateExercisePrescriptionDto
    {
        public int PhysiotherapistId { get; set; }
        public int TreatmentPlanId { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public string? Diagnosis { get; set; }
        public string? TargetGoal { get; set; }
        public List<CreateExercisePrescriptionDetailDto>? PrescriptionDetails { get; set; }
    }

    public class AddExerciseToPrescriptionDto
    {
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public int HoldSeconds { get; set; }
        public int FrequencyPerDay { get; set; }
        public int DurationWeeks { get; set; }
        public string Instructions { get; set; } = string.Empty;
    }

    public class UpdateExercisePrescriptionStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class ExercisePrescriptionFilterDto
    {
        public int? PatientId { get; set; }
        public int? PhysiotherapistId { get; set; }
        public int? TreatmentPlanId { get; set; }
        public string? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }
}
