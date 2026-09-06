using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class TreatmentType : BaseEntity
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DefaultDurationMinutes { get; set; } = 30;
        public decimal DefaultPrice { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
