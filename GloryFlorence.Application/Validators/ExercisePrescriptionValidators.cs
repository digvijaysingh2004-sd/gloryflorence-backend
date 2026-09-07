using System;
using System.Linq;
using FluentValidation;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Validators
{
    public class CreateExercisePrescriptionDetailDtoValidator : AbstractValidator<CreateExercisePrescriptionDetailDto>
    {
        public CreateExercisePrescriptionDetailDtoValidator()
        {
            RuleFor(x => x.ExerciseId)
                .GreaterThan(0).WithMessage("Valid exercise ID is required.");

            RuleFor(x => x.Sets)
                .GreaterThan(0).WithMessage("Sets must be greater than 0.");

            RuleFor(x => x.Repetitions)
                .GreaterThan(0).WithMessage("Repetitions must be greater than 0.");

            RuleFor(x => x.HoldSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("Hold seconds must be greater than or equal to 0.");

            RuleFor(x => x.FrequencyPerDay)
                .GreaterThan(0).WithMessage("Frequency per day must be greater than 0.");

            RuleFor(x => x.DurationWeeks)
                .GreaterThan(0).WithMessage("Duration in weeks must be greater than 0.");

            RuleFor(x => x.Instructions)
                .MaximumLength(1000).WithMessage("Instructions cannot exceed 1000 characters.");
        }
    }

    public class CreateExercisePrescriptionDtoValidator : AbstractValidator<CreateExercisePrescriptionDto>
    {
        private static readonly string[] AllowedStatuses = { "Active", "Completed", "Discontinued" };

        public CreateExercisePrescriptionDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("Valid patient ID is required.");

            RuleFor(x => x.PhysiotherapistId)
                .GreaterThan(0).WithMessage("Valid physiotherapist ID is required.");

            RuleFor(x => x.TreatmentPlanId)
                .GreaterThan(0).WithMessage("Valid treatment plan ID is required.");

            RuleFor(x => x.PrescriptionDate)
                .NotEmpty().WithMessage("Prescription date is required.");

            RuleFor(x => x.Instructions)
                .MaximumLength(2000).WithMessage("Instructions cannot exceed 2000 characters.");

            RuleFor(x => x.Status)
                .Must(s => string.IsNullOrEmpty(s) || AllowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}");

            RuleFor(x => x.PrescriptionDetails)
                .NotEmpty().WithMessage("At least one exercise must be prescribed.")
                .Must(details => details == null || details.Select(d => d.ExerciseId).Distinct().Count() == details.Count)
                .WithMessage("Prescription cannot contain duplicate exercises.");

            RuleForEach(x => x.PrescriptionDetails).SetValidator(new CreateExercisePrescriptionDetailDtoValidator());
        }
    }

    public class UpdateExercisePrescriptionDtoValidator : AbstractValidator<UpdateExercisePrescriptionDto>
    {
        private static readonly string[] AllowedStatuses = { "Active", "Completed", "Discontinued" };

        public UpdateExercisePrescriptionDtoValidator()
        {
            RuleFor(x => x.PhysiotherapistId)
                .GreaterThan(0).WithMessage("Valid physiotherapist ID is required.");

            RuleFor(x => x.TreatmentPlanId)
                .GreaterThan(0).WithMessage("Valid treatment plan ID is required.");

            RuleFor(x => x.PrescriptionDate)
                .NotEmpty().WithMessage("Prescription date is required.");

            RuleFor(x => x.Instructions)
                .MaximumLength(2000).WithMessage("Instructions cannot exceed 2000 characters.");

            RuleFor(x => x.Status)
                .Must(s => string.IsNullOrEmpty(s) || AllowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}");

            When(x => x.PrescriptionDetails != null, () =>
            {
                RuleFor(x => x.PrescriptionDetails!)
                    .Must(details => details.Select(d => d.ExerciseId).Distinct().Count() == details.Count)
                    .WithMessage("Prescription cannot contain duplicate exercises.");

                RuleForEach(x => x.PrescriptionDetails!).SetValidator(new CreateExercisePrescriptionDetailDtoValidator());
            });
        }
    }

    public class AddExerciseToPrescriptionDtoValidator : AbstractValidator<AddExerciseToPrescriptionDto>
    {
        public AddExerciseToPrescriptionDtoValidator()
        {
            RuleFor(x => x.ExerciseId)
                .GreaterThan(0).WithMessage("Valid exercise ID is required.");

            RuleFor(x => x.Sets)
                .GreaterThan(0).WithMessage("Sets must be greater than 0.");

            RuleFor(x => x.Repetitions)
                .GreaterThan(0).WithMessage("Repetitions must be greater than 0.");

            RuleFor(x => x.HoldSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("Hold seconds must be greater than or equal to 0.");

            RuleFor(x => x.FrequencyPerDay)
                .GreaterThan(0).WithMessage("Frequency per day must be greater than 0.");

            RuleFor(x => x.DurationWeeks)
                .GreaterThan(0).WithMessage("Duration in weeks must be greater than 0.");

            RuleFor(x => x.Instructions)
                .MaximumLength(1000).WithMessage("Instructions cannot exceed 1000 characters.");
        }
    }

    public class UpdateExercisePrescriptionStatusDtoValidator : AbstractValidator<UpdateExercisePrescriptionStatusDto>
    {
        private static readonly string[] AllowedStatuses = { "Active", "Completed", "Discontinued" };

        public UpdateExercisePrescriptionStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => AllowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}");
        }
    }
}
