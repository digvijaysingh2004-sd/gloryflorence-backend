using System;

namespace GloryFlorence.Application.DTOs
{
    public class PatientMedicalHistoryDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string TreatmentReceived { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}
