namespace GloryFlorence.Application.DTOs
{
    public class UpdateAppointmentTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
