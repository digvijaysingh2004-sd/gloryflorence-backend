using FluentValidation;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Validators
{
    public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
    {
        public CreateInvoiceDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("PatientId must be greater than 0.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one line item is required for an invoice.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Description)
                    .NotEmpty().WithMessage("Item description is required.");
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be at least 1.");
                item.RuleFor(i => i.UnitPrice)
                    .GreaterThanOrEqualTo(0).WithMessage("UnitPrice cannot be negative.");
            });
        }
    }
}
