
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CountryVMs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CountryValidators
{
    public class CountryCreateValidation : AbstractValidator<CountryCreateVM>
    {

        private readonly IStringLocalizer _localizer;
        private readonly ICountryService _countryService;

        public CountryCreateValidation(ICountryService countryService, IStringLocalizer<Resource> localizer)
        {
            _countryService = countryService;
            _localizer = localizer;

            RuleFor(s => s.Name)
              .NotEmpty()
              .WithMessage(@localizer[Messages.COUNTRY_NAME_NOT_EMPTY])
              .Matches(@"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$")
              .WithMessage(@localizer[Messages.COUNTRY_NAME_SHOULD_BE_STRING])
              .MaximumLength(30)
              .WithMessage(@localizer[Messages.COUNTRY_NAME_MAXIMUM_LENGTH])
              .MinimumLength(2)
              .WithMessage(@localizer[Messages.COUNTRY_NAME_MINIMUM_LENGTH])
              .Must(BeUniqueCountryName)
              .WithMessage(@localizer[Messages.COUNTRY_NAME_MUST_BE_UNIQUE]);
            
        }
        private bool BeUniqueCountryName(string countryName)
        {
            return !_countryService.CountryExist(countryName);
        }
    }
}
