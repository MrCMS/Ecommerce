using System;
using System.Collections.Generic;
using System.ServiceModel;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.PaypointService.API;
using MrCMS.Web.Apps.Ecommerce.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using Ninject;
using Ninject.Web.Common;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceApp : MrCMSApp
    {
        private static Dictionary<string, string> _salesChannelApps;
        public const string EcommerceAppName = "Ecommerce";
        public const string DefaultSalesChannel = "MrCMS";
        public const string NopCommerceSalesChannel = "NopCommerce";

        public override string AppName
        {
            get { return EcommerceAppName; }
        }

        public override string Version
        {
            get { return "0.4.1"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12; //ensure best ssl protocol is used

            kernel.Rebind<CartModel>().ToMethod(context => context.Kernel.Get<ICartBuilder>().BuildCart()).InRequestScope();
            kernel.Rebind<SECVPN>().ToMethod(context =>
            {
                var service = new SECVPNClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport),
                    new EndpointAddress("https://www.secpay.com/java-bin/services/SECCardService"));

                return service;
            });
        }

        public override IEnumerable<Type> BaseTypes
        {
            get
            {
                yield return typeof(DiscountLimitation);
                yield return typeof(DiscountApplication);
                yield return typeof(CartItemBasedDiscountApplication);
                yield return typeof(EcommerceSearchablePage);
                yield return typeof(RewardPointsHistory);
                yield return typeof(OrderRewardPointsHistory);
            }
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            EcommerceRouteConfig.RegisterRoutes(context);

            SetupSalesChannels();
        }

        private static void SetupSalesChannels()
        {
            _salesChannelApps = new Dictionary<string, string>();
            _salesChannelApps[DefaultSalesChannel] = EcommerceAppName;
            _salesChannelApps[NopCommerceSalesChannel] = EcommerceAppName;
            foreach (var appName in TypeHelper.GetAllConcreteTypesAssignableFrom<IEcommerceApp>())
            {
                var ecommerceApp = Activator.CreateInstance(appName) as IEcommerceApp;
                if (ecommerceApp != null)
                    foreach (var salesChannel in ecommerceApp.SalesChannels)
                        _salesChannelApps[salesChannel] = ecommerceApp.AppName;
            }
        }

        public static Dictionary<string, string> SalesChannelApps
        {
            get { return _salesChannelApps; }
        }

        public static IEnumerable<string> SalesChannels
        {
            get { return _salesChannelApps.Keys; }
        }

        protected override void AppendConfiguration(NHibernate.Cfg.Configuration configuration)
        {
        }

        public override IEnumerable<Type> Conventions
        {
            get { yield return typeof(TableNameConvention); }
        }
    }

    public interface IEcommerceApp
    {
        IEnumerable<string> SalesChannels { get; }
        string AppName { get; }
    }
}