
using BaSalesManagementApp.MVC.Models.CityVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CityValidators
{
    public class CityCreateValidation : AbstractValidator<CityCreateVM>
    {

        private readonly ICityService _cityService;
        public CityCreateValidation(ICityService cityService)
        {

            this._cityService = cityService;
            RuleFor(s => s.Name)
              .NotEmpty()
              .WithMessage(Messages.CITY_NAME_CANNOT_BE_EMPTY)
              .Matches(@"^[a-zA-ZçÇğĞıİöÖşŞüÜ]+$")
              .WithMessage(Messages.CITY_NAME_CANNOT_CONTAIN_NUMBER)
              .Must(BeUniqueCityName)
              .WithMessage(Messages.CITY_NAME_MUST_BE_UNIQUE);
            RuleFor(s => s.CountryId)
                .NotEmpty();
        }

        private bool BeUniqueCityName(string cityName)
        {
            return !_cityService.CityExist(cityName);
        }
    }



}
