using FluentValidation;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Validators
{
    public class CreatePatientAssessmentDtoValidator : AbstractValidator<CreatePatientAssessmentDto>
    {
        public CreatePatientAssessmentDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("Valid patient ID is required.");

            RuleFor(x => x.PhysiotherapistId)
                .GreaterThan(0).WithMessage("Valid physiotherapist ID is required.");

            RuleFor(x => x.AssessmentDate)
                .NotEmpty().WithMessage("Assessment date is required.");

            RuleFor(x => x.ChiefComplaint)
                .NotEmpty().WithMessage("Chief complaint is required.")
                .MaximumLength(1000).WithMessage("Chief complaint cannot exceed 1000 characters.");

            RuleFor(x => x.CurrentCondition)
                .MaximumLength(1000).WithMessage("Current condition cannot exceed 1000 characters.");

            RuleFor(x => x.PainLevel)
                .InclusiveBetween(0, 10).WithMessage("Pain level must be between 0 and 10.");

            RuleFor(x => x.Diagnosis)
                .MaximumLength(500).WithMessage("Diagnosis cannot exceed 500 characters.");

            RuleFor(x => x.ClinicalNotes)
                .MaximumLength(2000).WithMessage("Clinical notes cannot exceed 2000 characters.");

            RuleFor(x => x.Recommendations)
                .MaximumLength(2000).WithMessage("Recommendations cannot exceed 2000 characters.");
        }
    }

    public class UpdatePatientAssessmentDtoValidator : AbstractValidator<UpdatePatientAssessmentDto>
    {
        public UpdatePatientAssessmentDtoValidator()
        {
            RuleFor(x => x.PhysiotherapistId)
                .GreaterThan(0).WithMessage("Valid physiotherapist ID is required.");

            RuleFor(x => x.AssessmentDate)
                .NotEmpty().WithMessage("Assessment date is required.");

            RuleFor(x => x.ChiefComplaint)
                .NotEmpty().WithMessage("Chief complaint is required.")
                .MaximumLength(1000).WithMessage("Chief complaint cannot exceed 1000 characters.");

            RuleFor(x => x.CurrentCondition)
                .MaximumLength(1000).WithMessage("Current condition cannot exceed 1000 characters.");

            RuleFor(x => x.PainLevel)
                .InclusiveBetween(0, 10).WithMessage("Pain level must be between 0 and 10.");

            RuleFor(x => x.Diagnosis)
                .MaximumLength(500).WithMessage("Diagnosis cannot exceed 500 characters.");

            RuleFor(x => x.ClinicalNotes)
                .MaximumLength(2000).WithMessage("Clinical notes cannot exceed 2000 characters.");

            RuleFor(x => x.Recommendations)
                .MaximumLength(2000).WithMessage("Recommendations cannot exceed 2000 characters.");
        }
    }
}
