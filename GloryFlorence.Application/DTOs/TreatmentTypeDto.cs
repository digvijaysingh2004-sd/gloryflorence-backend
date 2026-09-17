using System;

namespace GloryFlorence.Application.DTOs
{
    public class TreatmentTypeDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DefaultDurationMinutes { get; set; }
        public int DurationMinutes => DefaultDurationMinutes;
        public decimal DefaultPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
