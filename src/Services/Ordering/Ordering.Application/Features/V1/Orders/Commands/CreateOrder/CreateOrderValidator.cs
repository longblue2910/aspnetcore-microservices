using FluentValidation;

namespace Ordering.Application.Features.V1.Orders
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(p => p.Username)
                .NotEmpty().WithMessage("{Username} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{Username} must not exeeced 50 characters.");
        }
    }
}
