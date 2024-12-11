using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.StockVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.StockValidators
{
    public class StockCreateValidation : AbstractValidator<StockCreateVM>
    {

        public StockCreateValidation()
        {


            RuleFor(x => x.Count)
                .NotEmpty().WithMessage(Messages.STOCK_EMPTY_VALIDATION)
                .GreaterThanOrEqualTo(0).WithMessage(Messages.STOCK_POSITIVE_VALIDATION)
                .Must(x => x % 1 == 0).WithMessage(Messages.STOCK_WHOLENUMBER_VALIDATION);

        }
    }
}
