using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MarketplaceWebService;
using MarketplaceWebServiceFeedsClasses;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceProducts;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Controllers;
using MrCMS.Web.Apps.Amazon.DbConfiguration;
using MrCMS.Web.Apps.Amazon.ModelBinders;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Website;
using NHibernate;
using Ninject;
using Ninject.Web.Common;

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
            kernel.Bind<MarketplaceWebServiceOrders.MarketplaceWebServiceOrders>().ToMethod(context =>
            {
                var amazonAppSettings = context.Kernel.Get<AmazonAppSettings>();
                var config = new MarketplaceWebServiceOrdersConfig { ServiceURL = amazonAppSettings.OrdersApiEndpoint };
                return new MarketplaceWebServiceOrdersClient
                        ("MrCMS", MrCMSApplication.AssemblyVersion, amazonAppSettings.AWSAccessKeyId, amazonAppSettings.SecretKey, config);
            }).InRequestScope();

            kernel.Bind<MarketplaceWebServiceProducts.MarketplaceWebServiceProducts>().ToMethod(context =>
            {
                var amazonAppSettings = context.Kernel.Get<AmazonAppSettings>();
                var config = new MarketplaceWebServiceProductsConfig { ServiceURL = amazonAppSettings.ProductsApiEndpoint };
                return new MarketplaceWebServiceProductsClient
                        ("MrCMS", MrCMSApplication.AssemblyVersion, amazonAppSettings.AWSAccessKeyId, amazonAppSettings.SecretKey, config);
            }).InRequestScope();
            kernel.Bind<MarketplaceWebService.MarketplaceWebService>().ToMethod(context =>
            {
                var amazonAppSettings = context.Kernel.Get<AmazonAppSettings>();
                var config = new MarketplaceWebServiceConfig { ServiceURL = amazonAppSettings.ProductsApiEndpoint };
                return new MarketplaceWebServiceClient (amazonAppSettings.AWSAccessKeyId, amazonAppSettings.SecretKey, "MrCMS", MrCMSApplication.AssemblyVersion, config);
            }).InRequestScope();
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapAreaRoute("Amazon Admin controllers", "Admin", "Admin/Apps/Amazon/{controller}/{action}/{id}",
                                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                                 new[] { typeof(SettingsController).Namespace });
            context.MapRoute("Sync Amazon Orders", "sync-amazon-orders",
                                 new { controller = "AmazonSync", action = "Sync", id = UrlParameter.Optional },
                                 new[] { typeof(AmazonSyncController).Namespace });
            
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            var configurationProvider = new ConfigurationProvider(new SettingService(session), site);

            var amazonAppSettings = configurationProvider.GetSiteSettings<AmazonAppSettings>();
            amazonAppSettings.FeedsApiVersion = "2009-01-01";
            amazonAppSettings.OrdersApiVersion = "2011-01-01";
            amazonAppSettings.ProductsApiVersion = "2011-10-01";
            amazonAppSettings.ApiEndpoint = "https://mws.amazonservices.co.uk/";
            amazonAppSettings.AmazonProductDetailsUrl = "http://www.amazon.co.uk/gp/product/";
            amazonAppSettings.AmazonOrderDetailsUrl = "https://sellercentral.amazon.co.uk/gp/orders-v2/details/?orderID=";
            amazonAppSettings.AmazonManageOrdersUrl = "https://sellercentral.amazon.co.uk/gp/orders-v2/list";
            amazonAppSettings.AmazonManageInventoryUrl = "https://sellercentral.amazon.co.uk/myi/search/ProductSummary";

            configurationProvider.SaveSettings(amazonAppSettings);

            var amazonSellerSettings = configurationProvider.GetSiteSettings<AmazonSellerSettings>();
            amazonSellerSettings.BarcodeIsOfType = StandardProductIDType.EAN;

            configurationProvider.SaveSettings(amazonSellerSettings);

            var folderLocation = string.Format("{0}/{1}/", CurrentRequestData.CurrentSite.Id, "amazon");
            var fileSystem = new FileSystem();
            fileSystem.CreateDirectory(folderLocation);
        }

        public override IEnumerable<Type> Conventions
        {
            get { yield return typeof(AmazonTableNameConvention); }
        }
    }
}