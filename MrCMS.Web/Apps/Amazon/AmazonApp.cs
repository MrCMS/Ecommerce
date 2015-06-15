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
using MrCMS.Web.Apps.Amazon.DbConfiguration;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Amazon.Tasks;
using MrCMS.Web.Apps.Ecommerce;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Website;
using NHibernate;
using Ninject;
using Ninject.Web.Common;

namespace MrCMS.Web.Apps.Amazon
{
    public class AmazonApp : MrCMSApp, IEcommerceApp
    {
        public const string SalesChannel = "Amazon";
        public const string AmazonAppName = "Amazon";

        public IEnumerable<string> SalesChannels { get { yield return SalesChannel; } }

        public override string AppName
        {
            get { return AmazonAppName; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        public override IEnumerable<Type> Conventions
        {
            get { yield return typeof(AmazonTableNameConvention); }
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
                return new MarketplaceWebServiceClient(amazonAppSettings.AWSAccessKeyId, amazonAppSettings.SecretKey, "MrCMS", MrCMSApplication.AssemblyVersion, config);
            }).InRequestScope();
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapAreaRoute("Amazon Admin controllers", "Admin", "Admin/Apps/Amazon/{controller}/{action}/{id}",
                                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                                 new[] { typeof(SettingsController).Namespace });
        }
    }
}