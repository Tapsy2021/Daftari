using FluentValidation;

namespace Daftari.ViewModel.Validations
{
    public class CustomerMailsVMValidator : AbstractValidator<CustomerMailsVM>
    {
        public CustomerMailsVMValidator()
        {
            RuleFor(message => message.Customers).NotEmpty().WithMessage("Please select Recipients.");
            RuleFor(message => message.Subject).NotEmpty().WithMessage("Please enter Email Subject");
            RuleFor(message => message.EmailMessage).NotEmpty().WithMessage("Please enter Email Message");
        }
    }
}