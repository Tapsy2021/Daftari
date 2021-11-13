using Daftari.AquaCards.Interfaces;
using FluentValidation;
using System;

namespace Daftari.AquaCards.Validatons
{
    public class CustomerValidator : AbstractValidator<ICustomer>
    {
        public CustomerValidator()
        {
            RuleFor(customer => customer.FullName).NotEmpty().When(customer => customer.MotherName == string.Empty || customer.MotherName == null || customer.FatherName == string.Empty || customer.FatherName == null).WithMessage("Please enter atleast one Guardian's Name");
            RuleFor(customer => customer.PrimaryPhone).NotEmpty();
            RuleFor(customer => customer.PrimaryPhone).Matches("([2,7,9])[0-9]{7}").WithMessage("Invalid Phone Number, Phone Number Format: (2, 7 or 9)xxxxxxx");
            RuleFor(customer => customer.PrimaryPhone).Length(8);
            RuleFor(customer => customer.CellPhone).Matches("([2,7,9])[0-9]{7}").WithMessage("Invalid Phone Number, Phone Number Format: (2, 7 or 9)xxxxxxx");
            RuleFor(customer => customer.CellPhone).Length(8);
            RuleFor(customer => customer.AlternatePhone).Matches("([2,7,9])[0-9]{7}").WithMessage("Invalid Phone Number, Phone Number Format: (2, 7 or 9)xxxxxxx");
            RuleFor(customer => customer.AlternatePhone).Length(8);
            RuleFor(customer => customer.EmailAddress).EmailAddress().WithMessage("A valid email address is required");
        }
    }
}