using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Web.Areas.Admin.Controllers;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Amazon
{
    public class AmazonApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Amazon"; }
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
            
        }
    }
}