using System;

namespace GloryFlorence.Application.DTOs
{
    public class TreatmentSessionDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PhysiotherapistId { get; set; }
        public string PhysiotherapistName { get; set; } = string.Empty;
        public int? TreatmentPlanId { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? PainLevelBefore { get; set; }
        public int? PainLevelAfter { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Assessment { get; set; } = string.Empty;
        public string TreatmentPerformed { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTreatmentSessionDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int PhysiotherapistId { get; set; }
        public int? TreatmentPlanId { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? PainLevelBefore { get; set; }
        public int? PainLevelAfter { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled, NoShow
        public string Assessment { get; set; } = string.Empty;
        public string TreatmentPerformed { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateTreatmentSessionDto
    {
        public int? TreatmentPlanId { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? PainLevelBefore { get; set; }
        public int? PainLevelAfter { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Assessment { get; set; } = string.Empty;
        public string TreatmentPerformed { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class CompleteTreatmentSessionDto
    {
        public int? PainLevelAfter { get; set; }
        public string TreatmentPerformed { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class TreatmentSessionFilterDto
    {
        public int? PatientId { get; set; }
        public int? AppointmentId { get; set; }
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
