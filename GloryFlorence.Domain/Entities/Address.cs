using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string StreetAddress { get; set; } = string.Empty;
        public int CityId { get; set; }
        public virtual City City { get; set; } = null!;
        public int StateId { get; set; }
        public virtual State State { get; set; } = null!;
        public int CountryId { get; set; }
        public virtual Country Country { get; set; } = null!;
        public string PostalCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
