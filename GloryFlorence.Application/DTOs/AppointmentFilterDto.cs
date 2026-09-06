using System;

namespace GloryFlorence.Application.DTOs
{
    public class AppointmentFilterDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? PatientId { get; set; }
        public int? PhysiotherapistId { get; set; }
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
