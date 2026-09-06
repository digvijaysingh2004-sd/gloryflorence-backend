using FluentValidation;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Validators
{
    public class CreateAppointmentTypeDtoValidator : AbstractValidator<CreateAppointmentTypeDto>
    {
        public CreateAppointmentTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Appointment type name is required.")
                .MaximumLength(100).WithMessage("Appointment type name must not exceed 100 characters.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
