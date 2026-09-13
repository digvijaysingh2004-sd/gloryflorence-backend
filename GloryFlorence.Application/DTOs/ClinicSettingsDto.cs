using System;
using System.Text.Json.Serialization;
using GloryFlorence.Application.Common.Converters;

namespace GloryFlorence.Application.DTOs
{
    public class ClinicSettingsDto
    {
        public int Id { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string? Tagline { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? TaxRegistrationNumber { get; set; }
        public string? CurrencySymbol { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan WorkingHoursStart { get; set; }
        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan WorkingHoursEnd { get; set; }
        public int AppointmentSlotDurationMinutes { get; set; }
        public bool AutoConfirmAppointments { get; set; }
        public bool EnableSmsNotifications { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateClinicSettingsDto
    {
        public string ClinicName { get; set; } = string.Empty;
        public string? Tagline { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? TaxRegistrationNumber { get; set; }
        public string? CurrencySymbol { get; set; }
        [JsonConverter(typeof(NullableFlexibleTimeSpanJsonConverter))]
        public TimeSpan? WorkingHoursStart { get; set; }
        [JsonConverter(typeof(NullableFlexibleTimeSpanJsonConverter))]
        public TimeSpan? WorkingHoursEnd { get; set; }
        public int? AppointmentSlotDurationMinutes { get; set; }
        public bool? AutoConfirmAppointments { get; set; }
        public bool? EnableSmsNotifications { get; set; }
        public bool? EnableEmailNotifications { get; set; }
    }
}
