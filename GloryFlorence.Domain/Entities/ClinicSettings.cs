using System;

namespace GloryFlorence.Domain.Entities
{
    public class ClinicSettings
    {
        public int Id { get; set; }
        public string ClinicName { get; set; } = "Glory Florence Physiotherapy Clinic";
        public string? Tagline { get; set; } = "Advanced Rehabilitation & Wellness";
        public string? Email { get; set; } = "info@gloryflorence.com";
        public string? Phone { get; set; } = "+1 (555) 234-5678";
        public string? Address { get; set; } = "123 Medical Center Boulevard";
        public string? City { get; set; } = "Healthcare City";
        public string? State { get; set; } = "NY";
        public string? Country { get; set; } = "United States";
        public string? PostalCode { get; set; } = "10001";
        public string? TaxRegistrationNumber { get; set; } = "TAX-99887766";
        public string? CurrencySymbol { get; set; } = "$";
        public TimeSpan WorkingHoursStart { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan WorkingHoursEnd { get; set; } = new TimeSpan(18, 0, 0);
        public int AppointmentSlotDurationMinutes { get; set; } = 30;
        public bool AutoConfirmAppointments { get; set; } = true;
        public bool EnableSmsNotifications { get; set; } = true;
        public bool EnableEmailNotifications { get; set; } = true;
        public DateTime? UpdatedAt { get; set; }
    }
}
