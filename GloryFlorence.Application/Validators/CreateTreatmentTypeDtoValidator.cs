using FluentValidation;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Validators
{
    public class CreateTreatmentTypeDtoValidator : AbstractValidator<CreateTreatmentTypeDto>
    {
        public CreateTreatmentTypeDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThanOrEqualTo(0).WithMessage("Valid Category ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Treatment name is required.")
                .MaximumLength(100).WithMessage("Treatment name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.DefaultDurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.");

            RuleFor(x => x.DefaultPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
        }
    }
}
