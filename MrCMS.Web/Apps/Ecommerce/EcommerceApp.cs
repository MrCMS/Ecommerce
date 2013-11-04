using System;
using System.Collections.Generic;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.PaypointService.API;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.DbConfiguration.Listeners;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using NHibernate;
using NHibernate.Event;
using Ninject;
using Ninject.Web.Common;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceApp : MrCMSApp
    {
        public const string EcommerceAppName = "Ecommerce";

        public override string AppName
        {
            get { return EcommerceAppName; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            kernel.Rebind<CartModel>().ToMethod(context => context.Kernel.Get<ICartBuilder>().BuildCart()).InRequestScope();
            kernel.Bind<SECVPN>().To<SECVPNClient>().InRequestScope();
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
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            EcommerceInstallation.InstallApp(session, model, site);
        }

        public override IEnumerable<Type> Conventions
        {
            get { yield return typeof(TableNameConvention); }
        }

        protected override void AppendConfiguration(NHibernate.Cfg.Configuration configuration)
        {
            configuration.AppendListeners(ListenerType.PostCommitUpdate, new IPostUpdateEventListener[]
                                                                             {
                                                                                 new BackInStockListener()
                                                                             });
        }
    }
}