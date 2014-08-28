using MrCMS.Entities.Documents.Layout;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupEcommerceSettings : ISetupEcommerceSettings
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IDocumentService _documentService;

        public SetupEcommerceSettings(IConfigurationProvider configurationProvider, IDocumentService documentService)
        {
            _configurationProvider = configurationProvider;
            _documentService = documentService;
        }

        public void Setup(MediaModel mediaModel)
        {
            var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
            var documentByUrl = _documentService.GetDocumentByUrl<Layout>("_EcommerceLayout");
            if (documentByUrl != null)
                siteSettings.DefaultLayoutId = documentByUrl.Id;
            siteSettings.ThemeName = "Ecommerce";
            _configurationProvider.SaveSettings(siteSettings);

            var ecommerceSettings = _configurationProvider.GetSiteSettings<EcommerceSettings>();
            ecommerceSettings.SearchProductsPerPage = "12,20,40";
            ecommerceSettings.PreviousPriceText = "Previous price";
            ecommerceSettings.DefaultNoProductImage = mediaModel.AwatiginImage.FileUrl;
            _configurationProvider.SaveSettings(ecommerceSettings);
        }
    }
}