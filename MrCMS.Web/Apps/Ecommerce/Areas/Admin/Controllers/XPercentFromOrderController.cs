using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class XPercentFromOrderController : MrCMSAppAdminController<EcommerceApp>
    {
        public PartialViewResult Fields(XPercentFromOrder xPercentFromOrder)
        {
            return PartialView(xPercentFromOrder);
        }
    }
}