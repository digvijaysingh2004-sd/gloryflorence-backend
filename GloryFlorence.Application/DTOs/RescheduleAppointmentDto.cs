using System;

namespace GloryFlorence.Application.DTOs
{
    public class RescheduleAppointmentDto
    {
        public DateTime NewAppointmentDate { get; set; }
        public TimeSpan NewStartTime { get; set; }
        public TimeSpan NewEndTime { get; set; }
        public string? Reason { get; set; }
    }
}
