using GloryFlorence.Domain.Common;

namespace GloryFlorence.Domain.Entities
{
    public class ExercisePrescriptionDetail : BaseEntity
    {
        public int PrescriptionId { get; set; }
        public virtual ExercisePrescription Prescription { get; set; } = null!;

        public int ExerciseId { get; set; }
        public virtual Exercise Exercise { get; set; } = null!;

        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public int HoldSeconds { get; set; }
        public int FrequencyPerDay { get; set; }
        public int DurationWeeks { get; set; }
        public string Instructions { get; set; } = string.Empty;
    }
}
