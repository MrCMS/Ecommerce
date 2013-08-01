using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Web.Apps.Ryness.Areas.Admin.Controllers;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Ryness
{
    public class RynessApp : MrCMSApp
    {
        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            //context.MapAreaRoute("Admin controllers", "Admin", "Admin/Apps/Ryness/{controller}/{action}/{id}",
            //                     new {controller = "Home", action = "Index", id = UrlParameter.Optional},
            //                     new[] {typeof (TestimonialController).Namespace});
        }

        public override string AppName
        {
            get { return "Ryness"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            
        }
    }
}