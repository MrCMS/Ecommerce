using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.PaypointService.API;
using MrCMS.Web.Apps.Ecommerce.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.DbConfiguration.Listeners;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;
using NHibernate;
using NHibernate.Event;
using Ninject;
using Ninject.Web.Common;
using SagePayMvc;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceApp : MrCMSApp
    {
        private static Dictionary<string, string> _salesChannelApps;
        public const string EcommerceAppName = "Ecommerce";
        public const string DefaultSalesChannel = "MrCMS";

        public override string AppName
        {
            get { return EcommerceAppName; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            kernel.Rebind<CartModel>().ToMethod(context => context.Kernel.Get<ICartBuilder>().BuildCart()).InRequestScope();
            kernel.Bind<SECVPN>().To<SECVPNClient>().InRequestScope();
            kernel.Bind<RequestContext>().ToMethod(context => context.Kernel.Get<HttpContextBase>().Request.RequestContext).InRequestScope();
            kernel.Rebind<IHttpRequestSender>().To<HttpRequestSender>().InRequestScope();
            kernel.Rebind<Configuration>().ToMethod(context =>
                                                        {
                                                            var configuration = context.Kernel.Get<SagePaySettings>().Configuration;
                                                            Configuration.Configure(configuration);
                                                            return configuration;
                                                        });
            kernel.Rebind<IUrlResolver>().ToMethod(context =>
                                                       {
                                                           var mrCMSSagePayUrlResolver =
                                                               context.Kernel.Get<MrCMSSagePayUrlResolver>();
                                                           UrlResolver.Initialize(() => mrCMSSagePayUrlResolver);
                                                           return mrCMSSagePayUrlResolver;
                                                       }).InSingletonScope();
            kernel.Rebind<ITransactionRegistrar>().ToMethod(context => new TransactionRegistrar(
                                                                           context.Kernel.Get<Configuration>(),
                                                                           context.Kernel.Get<IUrlResolver>(),
                                                                           context.Kernel.Get<IHttpRequestSender>())).InRequestScope();
        }

        public override IEnumerable<Type> BaseTypes
        {
            get
            {
                yield return typeof(DiscountLimitation);
                yield return typeof(DiscountApplication);
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
            foreach (var appName in TypeHelper.GetAllConcreteTypesAssignableFrom<IEcommerceApp>())
            {
                var ecommerceApp = Activator.CreateInstance(appName) as IEcommerceApp;
                if (ecommerceApp != null)
                    foreach (var salesChannel in ecommerceApp.SalesChannels)
                        _salesChannelApps[salesChannel] = ecommerceApp.AppName;
            }
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            EcommerceInstallation.InstallApp(session, model, site);
        }

        public override IEnumerable<Type> Conventions
        {
            get { yield return typeof(TableNameConvention); }
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
            configuration.AppendListeners(ListenerType.PostCommitUpdate, new IPostUpdateEventListener[]
                                                                             {
                                                                                 new BackInStockListener()
                                                                             });
        }
    }

    public interface IEcommerceApp
    {
        IEnumerable<string> SalesChannels { get; }
        string AppName { get; }
    }
}