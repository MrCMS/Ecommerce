using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Ecommerce"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapAreaRoute("Admin controllers", "Admin", "Admin/Apps/Ecommerce/{controller}/{action}/{id}",
                                 new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                                 new[] {typeof (ProductController).Namespace});
        }
    }
}