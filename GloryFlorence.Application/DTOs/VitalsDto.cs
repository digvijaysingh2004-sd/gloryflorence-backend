using System;

namespace GloryFlorence.Application.DTOs
{
    public class VitalsDto
    {
        public string? BloodPressure { get; set; }
        public int? HeartRate { get; set; }
        public double? WeightKg { get; set; }
        public double? HeightCm { get; set; }
        public double? Temperature { get; set; }
        public double? OxygenSaturation { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
