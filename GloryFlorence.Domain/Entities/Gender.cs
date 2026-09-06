using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class Gender : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
