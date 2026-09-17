namespace GloryFlorence.Application.DTOs
{
    public class CreateTreatmentTypeDto
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DefaultDurationMinutes { get; set; } = 30;
        public int DurationMinutes
        {
            get => DefaultDurationMinutes;
            set => DefaultDurationMinutes = value;
        }
        public decimal DefaultPrice { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
