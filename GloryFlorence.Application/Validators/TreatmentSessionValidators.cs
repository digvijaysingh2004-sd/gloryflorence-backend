using System;
using FluentValidation;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Validators
{
    public class CreateTreatmentSessionDtoValidator : AbstractValidator<CreateTreatmentSessionDto>
    {
        public CreateTreatmentSessionDtoValidator()
        {
            RuleFor(x => x.AppointmentId)
                .GreaterThan(0).WithMessage("Valid appointment ID is required.");

            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("Valid patient ID is required.");

            RuleFor(x => x.PhysiotherapistId)
                .GreaterThan(0).WithMessage("Valid physiotherapist ID is required.");

            RuleFor(x => x.SessionDate)
                .NotEmpty().WithMessage("Session date is required.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

            When(x => x.PainLevelBefore.HasValue, () =>
            {
                RuleFor(x => x.PainLevelBefore!.Value)
                    .InclusiveBetween(0, 10).WithMessage("Pain level before must be between 0 and 10.");
            });

            When(x => x.PainLevelAfter.HasValue, () =>
            {
                RuleFor(x => x.PainLevelAfter!.Value)
                    .InclusiveBetween(0, 10).WithMessage("Pain level after must be between 0 and 10.");
            });

            RuleFor(x => x.Assessment)
                .MaximumLength(1000).WithMessage("Assessment cannot exceed 1000 characters.");

            RuleFor(x => x.TreatmentPerformed)
                .MaximumLength(1000).WithMessage("Treatment performed cannot exceed 1000 characters.");

            RuleFor(x => x.Recommendations)
                .MaximumLength(1000).WithMessage("Recommendations cannot exceed 1000 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
        }
    }

    public class UpdateTreatmentSessionDtoValidator : AbstractValidator<UpdateTreatmentSessionDto>
    {
        public UpdateTreatmentSessionDtoValidator()
        {
            RuleFor(x => x.SessionDate)
                .NotEmpty().WithMessage("Session date is required.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

            When(x => x.PainLevelBefore.HasValue, () =>
            {
                RuleFor(x => x.PainLevelBefore!.Value)
                    .InclusiveBetween(0, 10).WithMessage("Pain level before must be between 0 and 10.");
            });

            When(x => x.PainLevelAfter.HasValue, () =>
            {
                RuleFor(x => x.PainLevelAfter!.Value)
                    .InclusiveBetween(0, 10).WithMessage("Pain level after must be between 0 and 10.");
            });

            RuleFor(x => x.Assessment)
                .MaximumLength(1000).WithMessage("Assessment cannot exceed 1000 characters.");

            RuleFor(x => x.TreatmentPerformed)
                .MaximumLength(1000).WithMessage("Treatment performed cannot exceed 1000 characters.");

            RuleFor(x => x.Recommendations)
                .MaximumLength(1000).WithMessage("Recommendations cannot exceed 1000 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
        }
    }

    public class CompleteTreatmentSessionDtoValidator : AbstractValidator<CompleteTreatmentSessionDto>
    {
        public CompleteTreatmentSessionDtoValidator()
        {
            When(x => x.PainLevelAfter.HasValue, () =>
            {
                RuleFor(x => x.PainLevelAfter!.Value)
                    .InclusiveBetween(0, 10).WithMessage("Pain level after must be between 0 and 10.");
            });

            RuleFor(x => x.TreatmentPerformed)
                .MaximumLength(1000).WithMessage("Treatment performed cannot exceed 1000 characters.");

            RuleFor(x => x.Recommendations)
                .MaximumLength(1000).WithMessage("Recommendations cannot exceed 1000 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
        }
    }
}
