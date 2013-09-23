using System.Collections.Generic;
using System.Web.Mvc;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.DbConfiguration;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Amazon
{
    public class AmazonApp : MrCMSApp
    {
        public const string AmazonAppName = "Amazon";

        public override string AppName
        {
            get { return AmazonAppName; }
        }

        protected override void RegisterServices(IKernel kernel)
        {

        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapAreaRoute("Amazon Admin controllers", "Admin", "Admin/Apps/Amazon/{controller}/{action}/{id}",
                                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                                 new[] { typeof(SettingsController).Namespace });
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            var configurationProvider = new ConfigurationProvider(new SettingService(session), site);

            var amazonAppSettings = configurationProvider.GetSiteSettings<AmazonAppSettings>();
            amazonAppSettings.FeedsApiVersion = "2009-01-01";
            amazonAppSettings.OrdersApiVersion = "2011-01-01";
            amazonAppSettings.ProductsApiVersion = "2011-10-01";
            amazonAppSettings.SignatureVersion = "2";
            amazonAppSettings.SignatureMethod = "HmacSHA256";
            amazonAppSettings.AppName = "MrCMS";
            amazonAppSettings.AppPath = "/";
            amazonAppSettings.AppVersion = MrCMSApplication.AssemblyVersion;
            amazonAppSettings.AppLanguage = "C#";
            amazonAppSettings.ApiEndpoint = "https://mws.amazonservices.com/";
            amazonAppSettings.AmazonOrderDetailsUrl = "https://sellercentral.amazon.com/gp/orders-v2/details/?orderID=";
            amazonAppSettings.AmazonManageOrdersUrl = "https://sellercentral.amazon.com/gp/orders-v2/list";
            amazonAppSettings.AmazonManageInventoryUrl = "https://sellercentral.amazon.com/myi/search/ProductSummary";

            configurationProvider.SaveSettings(amazonAppSettings);

            var amazonSellerSettings = configurationProvider.GetSiteSettings<AmazonSellerSettings>();
            amazonSellerSettings.BarcodeIsOfType = StandardProductIDType.EAN;

            configurationProvider.SaveSettings(amazonSellerSettings);

            var folderLocation = string.Format("{0}/{1}/", CurrentRequestData.CurrentSite.Id,"amazon");
            var fileSystem = new FileSystem();
            fileSystem.CreateDirectory(folderLocation);
        }

        public override IEnumerable<System.Type> Conventions
        {
            get { yield return typeof(AmazonTableNameConvention); }
        }
    }
}