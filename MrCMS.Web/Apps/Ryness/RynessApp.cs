using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Apps;

namespace MrCMS.Web.Apps.Ryness
{
    public class RynessApp : MrCMSApp
    {
        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }

        public override string AppName
        {
            get { return "Ryness"; }
        }

        protected override void RegisterServices(Ninject.IKernel kernel)
        {
        }

        protected override void OnInstallation(NHibernate.ISession session, Installation.InstallModel model, Entities.Multisite.Site site)
        {
        }
    }
}