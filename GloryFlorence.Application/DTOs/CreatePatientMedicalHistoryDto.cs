using System;

namespace GloryFlorence.Application.DTOs
{
    public class CreatePatientMedicalHistoryDto
    {
        public string Diagnosis { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string TreatmentReceived { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public string Remarks { get; set; } = string.Empty;
    }
}
