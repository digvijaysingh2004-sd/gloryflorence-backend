using System;

namespace GloryFlorence.Application.DTOs
{
    public class UpdatePatientDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? Status { get; set; }
        public string MedicalHistory { get; set; } = string.Empty;

        // Vitals fields
        public string? BloodPressure { get; set; }
        public int? HeartRate { get; set; }
        public double? WeightKg { get; set; }
        public double? HeightCm { get; set; }
        public double? Temperature { get; set; }
        public double? OxygenSaturation { get; set; }
    }
}
