using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class XPercentFromItemsController : MrCMSAppAdminController<EcommerceApp>
    {
        public PartialViewResult Fields(XPercentFromItems xPercentFromItems)
        {
            return PartialView(xPercentFromItems);
        }
    }
}