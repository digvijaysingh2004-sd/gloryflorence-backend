using System;
using System.Text.Json.Serialization;
using GloryFlorence.Application.Common.Converters;

namespace GloryFlorence.Application.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhoneNumber { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;

        public int PhysiotherapistId { get; set; }
        public string PhysiotherapistName { get; set; } = string.Empty;
        public string PhysiotherapistEmail { get; set; } = string.Empty;

        public int AppointmentTypeId { get; set; }
        public string AppointmentTypeName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }

        public DateTime AppointmentDate { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan StartTime { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan EndTime { get; set; }

        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
        public string? Room { get; set; }
        public decimal? Fee { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
