using MrCMS.Entities.Documents.Layout;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupEcommerceSettings : ISetupEcommerceSettings
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IGetDocumentByUrl<Layout> _getByUrl;

        public SetupEcommerceSettings(IConfigurationProvider configurationProvider, IGetDocumentByUrl<Layout> getByUrl)
        {
            _configurationProvider = configurationProvider;
            _getByUrl = getByUrl;
        }

        public void Setup(MediaModel mediaModel)
        {
            var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
            var documentByUrl = _getByUrl.GetByUrl("_ContentLayout");
            if (documentByUrl != null)
                siteSettings.DefaultLayoutId = documentByUrl.Id;
            siteSettings.ThemeName = "Ecommerce";
            _configurationProvider.SaveSettings(siteSettings);

            var ecommerceSettings = _configurationProvider.GetSiteSettings<EcommerceSettings>();
            ecommerceSettings.SearchProductsPerPage = "12,20,40";
            ecommerceSettings.PreviousPriceText = "Was";
            ecommerceSettings.DefaultNoProductImage = mediaModel.AwatiginImage.FileUrl;
            ecommerceSettings.EnableWishlists = true;

            ecommerceSettings.ProductUrl = "product/{0}";
            ecommerceSettings.CategoryUrl = "category/{0}";
            ecommerceSettings.BrandUrl = "brand/{0}";

            _configurationProvider.SaveSettings(ecommerceSettings);

            var paymentSettings = _configurationProvider.GetSiteSettings<PaymentSettings>();
            paymentSettings.CashOnDeliveryEnabled = true;
            _configurationProvider.SaveSettings(paymentSettings);
        }
    }
}