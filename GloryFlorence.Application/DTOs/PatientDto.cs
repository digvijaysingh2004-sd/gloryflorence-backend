using System;
using System.Collections.Generic;

namespace GloryFlorence.Application.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string MRN { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Name => $"{FirstName} {LastName}".Trim();
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; } = "Active";
        public string MedicalHistory { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }

        public VitalsDto? Vitals { get; set; }
    }

    public class PatientDetailsDto : PatientDto
    {
        public List<PatientMedicalHistoryDto> MedicalHistories { get; set; } = new();
        public List<PatientDocumentDto> Documents { get; set; } = new();
        public List<AppointmentDto> Appointments { get; set; } = new();
        public List<PatientAssessmentDto> Assessments { get; set; } = new();
        public List<TreatmentPlanDto> TreatmentPlans { get; set; } = new();
        public List<ExercisePrescriptionDto> Prescriptions { get; set; } = new();
        public List<InvoiceDto> Invoices { get; set; } = new();
    }
}
