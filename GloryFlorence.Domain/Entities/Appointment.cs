using System;
using System.Collections.Generic;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int PhysiotherapistId { get; set; }
        public User Physiotherapist { get; set; } = null!;

        public int AppointmentTypeId { get; set; }
        public AppointmentType AppointmentType { get; set; } = null!;

        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, Completed, Cancelled, Rescheduled, No Show
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
        public string? Room { get; set; }
        public decimal? Fee { get; set; }

        // Optional legacy TherapistId for backwards-compatibility
        public int? TherapistId { get; set; }
        public Therapist? Therapist { get; set; }

        // Navigation properties
        public ICollection<TreatmentSession> TreatmentSessions { get; set; } = new List<TreatmentSession>();
    }
}
