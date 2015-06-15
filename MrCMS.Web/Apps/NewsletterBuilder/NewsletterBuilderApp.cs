using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using Ninject;

namespace MrCMS.Web.Apps.NewsletterBuilder
{
    public class NewsletterBuilderApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "NewsletterBuilder"; }
        }

        public override string Version
        {
            get { return "0.2"; }
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapAreaRoute("Newsletter Admin controllers", "Admin",
                "Admin/Apps/NewsletterBuilder/{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] {typeof (NewsletterController).Namespace});
        }

        protected override void RegisterServices(IKernel kernel)
        {
        }

        public override IEnumerable<Type> BaseTypes
        {
            get
            {
                yield return typeof(ContentItem);
                yield return typeof(ContentItemTemplateData);
            }
        }
    }
}