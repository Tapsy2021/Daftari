using FluentValidation;

namespace Daftari.ViewModel.Validations
{
    public class CustomerSMSVMValidator : AbstractValidator<CustomerSMSVM>
    {
        public CustomerSMSVMValidator()
        {
            RuleFor(message => message.Customers).NotEmpty().WithMessage("Please select Recipients.");
            RuleFor(message => message.Message).NotEmpty().WithMessage("Please enter SMS Message");
        }
    }


}