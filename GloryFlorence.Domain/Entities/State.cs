using System.Collections.Generic;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class State : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public virtual Country Country { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
    }
}
