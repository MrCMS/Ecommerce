using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ItemHasSKUController : MrCMSAppAdminController<EcommerceApp>
    {
        public PartialViewResult Fields(ItemHasSKU itemHasSKU)
        {
            return PartialView(itemHasSKU);
        }
    }
}