using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int StateId { get; set; }
        public virtual State State { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
