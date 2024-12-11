
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CompanyValidators
{
    public class CompanyUpdateValidation : AbstractValidator<CompanyUpdateVM>
    {

        public CompanyUpdateValidation()
        {


            RuleFor(s => s.Name)
                .NotEmpty()
                .WithMessage(Messages.COMPANY_NAME_NOT_EMPTY)
                .MaximumLength(128)
                .WithMessage(Messages.COMPANY_NAME_MAXIMUM_LENGTH)
                .MinimumLength(2)
                .WithMessage(Messages.COMPANY_NAME_MINIMUM_LENGTH);

            RuleFor(s => s.Address)
                .NotEmpty()
                .WithMessage(Messages.COMPANY_ADDRESS_NOT_EMPTY)
                .MaximumLength(128)
                .WithMessage(Messages.COMPANY_ADDRESS_MAXIMUM_LENGTH)
                .MinimumLength(2)
                .WithMessage(Messages.COMPANY_ADDRESS_MINIMUM_LENGTH);

            RuleFor(s => s.Phone)
                .NotEmpty()
                .WithMessage(Messages.COMPANY_PHONE_NOT_EMPTY)
                .MinimumLength(10)
                .WithMessage(Messages.COMPANY_PHONE_MINIMUM_LENGTH)
                .MaximumLength(40)
                .WithMessage(Messages.COMPANY_PHONE_MAXIMUM_LENGTH)
                .Matches(@"^\+?[0-9]+$")
                .WithMessage(Messages.COMPANY_MATCHES);
        }
    }
}
