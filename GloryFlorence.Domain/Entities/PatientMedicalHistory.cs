using System;
using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class PatientMedicalHistory : BaseEntity
    {
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;
        public string Diagnosis { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string TreatmentReceived { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public string Remarks { get; set; } = string.Empty;
    }
}
