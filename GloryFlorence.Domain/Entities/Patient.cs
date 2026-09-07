using System;
using System.Collections.Generic;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MedicalHistory { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<PatientMedicalHistory> MedicalHistories { get; set; } = new List<PatientMedicalHistory>();
        public virtual ICollection<PatientDocument> Documents { get; set; } = new List<PatientDocument>();
        public virtual ICollection<PatientAssessment> Assessments { get; set; } = new List<PatientAssessment>();
        public virtual ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
        public virtual ICollection<TreatmentSession> TreatmentSessions { get; set; } = new List<TreatmentSession>();
        public virtual ICollection<ExercisePrescription> ExercisePrescriptions { get; set; } = new List<ExercisePrescription>();
    }
}
