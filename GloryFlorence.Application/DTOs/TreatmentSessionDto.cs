using System;
using System.Text.Json.Serialization;
using GloryFlorence.Application.Common.Converters;

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
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan StartTime { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan EndTime { get; set; }
        public int? PainLevelBefore { get; set; }
        public int? PrePainLevel => PainLevelBefore;
        public int? PainLevelAfter { get; set; }
        public int? PostPainLevel => PainLevelAfter;
        public string Status { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Assessment { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string TreatmentPerformed { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Recommendations { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Notes { get; set; } = string.Empty;
        public string PatientFeedback => Notes;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? ModalitiesConducted { get; set; }
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? PatientTolerance { get; set; }
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? NextSessionPlan { get; set; }
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
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan StartTime { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan EndTime { get; set; }
        public int? PainLevelBefore { get; set; }
        public int? PainLevelAfter { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled, NoShow
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Assessment { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string TreatmentPerformed { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Recommendations { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Notes { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? ModalitiesConducted { get; set; }
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? PatientTolerance { get; set; }
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? NextSessionPlan { get; set; }
    }

    public class UpdateTreatmentSessionDto
    {
        public int? TreatmentPlanId { get; set; }
        public DateTime SessionDate { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan StartTime { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan EndTime { get; set; }
        public int? PainLevelBefore { get; set; }
        public int? PainLevelAfter { get; set; }
        public string Status { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Assessment { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string TreatmentPerformed { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Recommendations { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string Notes { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? ModalitiesConducted { get; set; }
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? PatientTolerance { get; set; }
        [JsonConverter(typeof(FlexibleStringJsonConverter))]
        public string? NextSessionPlan { get; set; }
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
