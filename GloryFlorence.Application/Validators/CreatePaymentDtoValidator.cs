using FluentValidation;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Validators
{
    public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentDtoValidator()
        {
            RuleFor(x => x.AmountPaid)
                .GreaterThan(0).WithMessage("AmountPaid must be greater than 0.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("PaymentMethod is required.");
        }
    }
}
