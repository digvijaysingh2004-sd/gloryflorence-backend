using System;
using System.Text.Json.Serialization;
using GloryFlorence.Application.Common.Converters;

namespace GloryFlorence.Application.DTOs
{
    public class RescheduleAppointmentDto
    {
        public DateTime NewAppointmentDate { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan NewStartTime { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan NewEndTime { get; set; }
        public string? Reason { get; set; }
    }
}
