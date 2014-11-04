using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation
{
    public class MobileFriendlyNavigationApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "MobileFriendlyNavigation"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("MFNav GetChildren", "MobileFriendlyNavigation/GetChildNodes", new {controller = "MobileFriendlyNavigation", action = "GetChildNodes"});
        }

        protected override void RegisterServices(IKernel kernel)
        {
        }

    }
}