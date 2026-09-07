using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class Exercise : BaseEntity
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<ExercisePrescriptionDetail> PrescriptionDetails { get; set; } = new List<ExercisePrescriptionDetail>();
    }
}
