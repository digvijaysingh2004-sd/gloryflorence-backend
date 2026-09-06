using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
