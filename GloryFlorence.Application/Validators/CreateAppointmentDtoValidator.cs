using FluentValidation;
using GloryFlorence.Application.DTOs;
using System;

namespace GloryFlorence.Application.Validators
{
    public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
    {
        public CreateAppointmentDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("Valid patient ID is required.");

            RuleFor(x => x.PhysiotherapistId)
                .GreaterThan(0).WithMessage("Valid physiotherapist ID is required.");

            RuleFor(x => x.AppointmentTypeId)
                .GreaterThan(0).WithMessage("Valid appointment type ID is required.");

            RuleFor(x => x.AppointmentDate)
                .NotEmpty().WithMessage("Appointment date is required.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

            RuleFor(x => x.Reason)
                .MaximumLength(250).WithMessage("Reason must not exceed 250 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
        }
    }
}
