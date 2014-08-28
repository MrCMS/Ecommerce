using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupCurrency : ISetupCurrency
    {
        private readonly ICurrencyService _currencyService;
        private readonly ITaxRateManager _taxRateManager;

        public SetupCurrency(ICurrencyService currencyService, ITaxRateManager taxRateManager)
        {
            _currencyService = currencyService;
            _taxRateManager = taxRateManager;
        }

        public void Setup()
        {
            //add currency
            var britishCurrency = new Entities.Currencies.Currency
            {
                Name = "British Pound",
                Code = "GBP",
                Format = "£0.00"
            };
            _currencyService.Add(britishCurrency);

            var taxRate = new TaxRate
            {
                Name = "VAT",
                Code = "S",
                Percentage = 20m
            };
            _taxRateManager.Add(taxRate);
        }
    }
}