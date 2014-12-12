using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CartSubtotalGreaterThanXController : MrCMSAppAdminController<EcommerceApp>
    {
        public PartialViewResult Fields(CartSubtotalGreaterThanX cartSubtotalGreaterThanX)
        {
            return PartialView(cartSubtotalGreaterThanX);
        }
    }
}