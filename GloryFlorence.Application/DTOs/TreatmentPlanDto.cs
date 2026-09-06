using System;
using System.Collections.Generic;

namespace GloryFlorence.Application.DTOs
{
    public class TreatmentPlanDetailDto
    {
        public int Id { get; set; }
        public int TreatmentPlanId { get; set; }
        public int TreatmentTypeId { get; set; }
        public string TreatmentTypeName { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public int NumberOfSessions { get; set; }
    }

    public class CreateTreatmentPlanDetailDto
    {
        public int TreatmentTypeId { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public int NumberOfSessions { get; set; }
    }

    public class TreatmentPlanDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PhysiotherapistId { get; set; }
        public string PhysiotherapistName { get; set; } = string.Empty;
        public int AssessmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int NumberOfSessions { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TreatmentPlanDetailDto> Details { get; set; } = new List<TreatmentPlanDetailDto>();
    }

    public class CreateTreatmentPlanDto
    {
        public int PatientId { get; set; }
        public int PhysiotherapistId { get; set; }
        public int AssessmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int NumberOfSessions { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft"; // Draft, Active, Completed, Discontinued
        public List<CreateTreatmentPlanDetailDto> Details { get; set; } = new List<CreateTreatmentPlanDetailDto>();
    }

    public class UpdateTreatmentPlanDto
    {
        public int PhysiotherapistId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int NumberOfSessions { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<CreateTreatmentPlanDetailDto>? Details { get; set; }
    }

    public class UpdateTreatmentPlanStatusDto
    {
        public string Status { get; set; } = string.Empty; // Draft, Active, Completed, Discontinued
    }

    public class TreatmentPlanFilterDto
    {
        public int? PatientId { get; set; }
        public int? PhysiotherapistId { get; set; }
        public string? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }
}
