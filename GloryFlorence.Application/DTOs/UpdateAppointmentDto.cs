using System;

namespace GloryFlorence.Application.DTOs
{
    public class UpdateAppointmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int PhysiotherapistId { get; set; }
        public int AppointmentTypeId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
