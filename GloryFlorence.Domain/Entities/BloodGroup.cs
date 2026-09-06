using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class BloodGroup : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
