using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Web.Apps.Stats.Controllers;
using MrCMS.Web.Apps.Stats.Filters;
using Ninject;

namespace MrCMS.Web.Apps.Stats
{
    public class StatsApp : MrCMSApp
    {
        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Analytics handler", "analytics/log-page-view",
               new { controller = "Analytics", action = "LogPageView" },
               new[] { typeof(AnalyticsController).Namespace });
        }

        public override string AppName
        {
            get { return "Stats"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            GlobalFilters.Filters.Add(new AnalyticsSessionFilter());
        }
    }
}