
using BaSalesManagementApp.MVC.Models.CityVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CityValidators
{
    public class CityUpdateValidation : AbstractValidator<CityUpdateVM>
    {


        public CityUpdateValidation()
        {


            RuleFor(s => s.Name)
              .NotEmpty()
              .WithMessage(Messages.CITY_NAME_CANNOT_BE_EMPTY)
              .Matches(@"^[a-zA-Z\s]*$")
              .WithMessage(Messages.CITY_NAME_CANNOT_CONTAIN_NUMBER);
        }
    }
}
