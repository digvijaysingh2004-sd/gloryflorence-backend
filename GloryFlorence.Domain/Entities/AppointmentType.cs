using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class AppointmentType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int DurationMinutes { get; set; } = 30;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
