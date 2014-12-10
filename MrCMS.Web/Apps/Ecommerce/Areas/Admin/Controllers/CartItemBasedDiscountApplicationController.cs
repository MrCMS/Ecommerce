using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CartItemBasedDiscountApplicationController : MrCMSAppAdminController<EcommerceApp>
    {
        public PartialViewResult Fields()
        {
            // this is done so that is doesn't whinge at runtime about not being able to bind application because it's abstract
            object application = RouteData.Values["application"];
            return PartialView(application);
        }
    }
}