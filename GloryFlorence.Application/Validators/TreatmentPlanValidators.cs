using System;
using System.Linq;
using FluentValidation;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Validators
{
    public class CreateTreatmentPlanDetailDtoValidator : AbstractValidator<CreateTreatmentPlanDetailDto>
    {
        public CreateTreatmentPlanDetailDtoValidator()
        {
            RuleFor(x => x.TreatmentTypeId)
                .GreaterThan(0).WithMessage("Valid treatment type ID is required.");

            RuleFor(x => x.Frequency)
                .MaximumLength(100).WithMessage("Frequency cannot exceed 100 characters.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).WithMessage("Duration in minutes must be greater than 0.");

            RuleFor(x => x.NumberOfSessions)
                .GreaterThan(0).WithMessage("Number of sessions must be greater than 0.");

            RuleFor(x => x.Instructions)
                .MaximumLength(1000).WithMessage("Instructions cannot exceed 1000 characters.");
        }
    }

    public class CreateTreatmentPlanDtoValidator : AbstractValidator<CreateTreatmentPlanDto>
    {
        private static readonly string[] AllowedStatuses = { "Draft", "Active", "Completed", "Discontinued" };

        public CreateTreatmentPlanDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("Valid patient ID is required.");

            RuleFor(x => x.PhysiotherapistId)
                .GreaterThan(0).WithMessage("Valid physiotherapist ID is required.");

            RuleFor(x => x.AssessmentId)
                .GreaterThan(0).WithMessage("Valid assessment ID is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.ExpectedEndDate)
                .NotEmpty().WithMessage("Expected end date is required.")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Expected end date must be on or after start date.");

            RuleFor(x => x.NumberOfSessions)
                .GreaterThan(0).WithMessage("Number of sessions must be greater than 0.");

            RuleFor(x => x.Goal)
                .MaximumLength(1000).WithMessage("Goal cannot exceed 1000 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.");

            RuleFor(x => x.Status)
                .Must(s => string.IsNullOrEmpty(s) || AllowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}");

            RuleForEach(x => x.Details).SetValidator(new CreateTreatmentPlanDetailDtoValidator());
        }
    }

    public class UpdateTreatmentPlanDtoValidator : AbstractValidator<UpdateTreatmentPlanDto>
    {
        private static readonly string[] AllowedStatuses = { "Draft", "Active", "Completed", "Discontinued" };

        public UpdateTreatmentPlanDtoValidator()
        {
            RuleFor(x => x.PhysiotherapistId)
                .GreaterThan(0).WithMessage("Valid physiotherapist ID is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.ExpectedEndDate)
                .NotEmpty().WithMessage("Expected end date is required.")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Expected end date must be on or after start date.");

            RuleFor(x => x.NumberOfSessions)
                .GreaterThan(0).WithMessage("Number of sessions must be greater than 0.");

            RuleFor(x => x.Goal)
                .MaximumLength(1000).WithMessage("Goal cannot exceed 1000 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.");

            RuleFor(x => x.Status)
                .Must(s => string.IsNullOrEmpty(s) || AllowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}");

            When(x => x.Details != null, () =>
            {
                RuleForEach(x => x.Details!).SetValidator(new CreateTreatmentPlanDetailDtoValidator());
            });
        }
    }

    public class UpdateTreatmentPlanStatusDtoValidator : AbstractValidator<UpdateTreatmentPlanStatusDto>
    {
        private static readonly string[] AllowedStatuses = { "Draft", "Active", "Completed", "Discontinued" };

        public UpdateTreatmentPlanStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => AllowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}");
        }
    }
}
