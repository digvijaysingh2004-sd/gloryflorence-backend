using System;
using System.Text.Json.Serialization;
using GloryFlorence.Application.Common.Converters;

namespace GloryFlorence.Application.DTOs
{
    public class CreateAppointmentDto
    {
        public int PatientId { get; set; }
        public int PhysiotherapistId { get; set; }
        public int AppointmentTypeId { get; set; }
        public DateTime AppointmentDate { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan StartTime { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string? Room { get; set; }
        public decimal? Fee { get; set; }
    }
}
