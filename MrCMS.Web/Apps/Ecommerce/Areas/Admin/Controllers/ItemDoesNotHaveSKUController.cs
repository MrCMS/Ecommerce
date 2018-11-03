using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ItemDoesNotHaveSKUController : MrCMSAppAdminController<EcommerceApp>
    {
        public PartialViewResult Fields(ItemDoesNotHaveSKU itemDoesNotHaveSKU)
        {
            return PartialView(itemDoesNotHaveSKU);
        }
    }
}