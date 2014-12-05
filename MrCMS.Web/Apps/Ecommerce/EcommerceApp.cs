using System;
using System.Collections.Generic;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.DbConfiguration.Listeners;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using NHibernate;
using NHibernate.Event;
using Ninject;
using Ninject.Web.Common;

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

        public override string Version
        {
            get { return "0.2.3"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            kernel.Rebind<CartModel>().ToMethod(context => context.Kernel.Get<ICartBuilder>().BuildCart()).InRequestScope();
            kernel.Rebind<IStatelessSession>()
                .ToMethod(context => context.Kernel.Get<ISessionFactory>().OpenStatelessSession());
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
        }

        public override IEnumerable<Type> BaseTypes
        {
            get
            {
                yield return typeof(DiscountLimitation);
                yield return typeof(DiscountApplication);
                yield return typeof(CartItemBasedDiscountApplication);
                yield return typeof(EcommerceSearchablePage);
                yield return typeof(ContentItem);
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
            foreach (var appName in TypeHelper.GetAllConcreteTypesAssignableFrom<IEcommerceApp>())
            {
                var ecommerceApp = Activator.CreateInstance(appName) as IEcommerceApp;
                if (ecommerceApp != null)
                    foreach (var salesChannel in ecommerceApp.SalesChannels)
                        _salesChannelApps[salesChannel] = ecommerceApp.AppName;
            }
        }

        //protected override void OnInstallation(ISession session, InstallModel model, Site site)
        //{
        //    EcommerceInstallation.InstallApp(session, model, site);
        //}

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