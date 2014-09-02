using MrCMS.Installation;
using MrCMS.Web.Apps.Ecommerce.Installation;
using MrCMS.Web.Apps.Ecommerce.Installation.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceAppInstallation : IOnInstallation
    {
        private readonly ISetupEcommerceLayouts _setupEcommerceLayouts;
        private readonly ISetupEcommerceMedia _setupEcommerceMedia;
        private readonly ISetupBaseDocuments _setupBaseDocuments;
        private readonly ISetupEcommerceSettings _setupEcommerceSettings;
        private readonly ISetupCurrency _setupCurrency;
        private readonly IImportDummyCategories _importDummyCategories;
        private readonly IImportDummyProducts _importDummyProducts;
        private readonly ISetupEcommerceWidgets _setupEcommerceWidgets;
        private readonly IIndexSetup _indexSetup;

        public EcommerceAppInstallation(ISetupEcommerceLayouts setupEcommerceLayouts,
            ISetupEcommerceMedia setupEcommerceMedia, ISetupBaseDocuments setupBaseDocuments,
            ISetupEcommerceSettings setupEcommerceSettings, ISetupCurrency setupCurrency,
            IImportDummyCategories importDummyCategories, IImportDummyProducts importDummyProducts,
            ISetupEcommerceWidgets setupEcommerceWidgets, IIndexSetup indexSetup)
        {
            _setupEcommerceLayouts = setupEcommerceLayouts;
            _setupEcommerceMedia = setupEcommerceMedia;
            _setupBaseDocuments = setupBaseDocuments;
            _setupEcommerceSettings = setupEcommerceSettings;
            _setupCurrency = setupCurrency;
            _importDummyCategories = importDummyCategories;
            _importDummyProducts = importDummyProducts;
            _setupEcommerceWidgets = setupEcommerceWidgets;
            _indexSetup = indexSetup;
        }

        public int Priority
        {
            get { return 100; }
        }

        public void Install(InstallModel model)
        {
            var mediaModel = _setupEcommerceMedia.Setup();
            var layoutModel = _setupEcommerceLayouts.Setup(mediaModel);
            var pageModel = _setupBaseDocuments.Setup(mediaModel);
            _setupEcommerceSettings.Setup(mediaModel);
            _setupCurrency.Setup();
            _importDummyCategories.Import(mediaModel);
            _importDummyProducts.Import();
            _setupEcommerceWidgets.Setup(pageModel, mediaModel, layoutModel);
            _indexSetup.ReIndex();
        }

    }

}