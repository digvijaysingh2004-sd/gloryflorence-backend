namespace GloryFlorence.Application.DTOs
{
    public class UpdateTreatmentTypeDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DefaultDurationMinutes { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool IsActive { get; set; }
    }
}
