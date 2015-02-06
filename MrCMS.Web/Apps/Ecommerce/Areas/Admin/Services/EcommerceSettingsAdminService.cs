using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class EcommerceSettingsAdminService : IEcommerceSettingsAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ICurrencyService _currencyService;

        public EcommerceSettingsAdminService(ICurrencyService currencyService,
            IConfigurationProvider configurationProvider)
        {
            _currencyService = currencyService;
            _configurationProvider = configurationProvider;
        }

        public List<SelectListItem> GetCurrencyOptions()
        {
            EcommerceSettings ecommerceSettings = GetSettings();
            return _currencyService.Options(ecommerceSettings.CurrencyId);
        }

        public EcommerceSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<EcommerceSettings>();
        }

        public void SaveSettings(EcommerceSettings ecommerceSettings)
        {
            _configurationProvider.SaveSettings(ecommerceSettings);
        }

        public List<SelectListItem> GetDefaultSortOptions()
        {
            return new List<ProductSearchSort>
            {
                ProductSearchSort.MostPopular,
                ProductSearchSort.Latest,
                ProductSearchSort.NameAToZ,
                ProductSearchSort.NameZToA,
                ProductSearchSort.PriceLowToHigh,
                ProductSearchSort.PriceHighToLow
            }.BuildSelectItemList(sort => sort.GetDescription(), sort => sort.ToString(), emptyItem: null);
        }
    }
}