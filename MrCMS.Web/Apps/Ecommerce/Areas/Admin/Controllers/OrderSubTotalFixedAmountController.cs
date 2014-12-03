using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderSubTotalFixedAmountController : MrCMSAppAdminController<EcommerceApp>
    {
        public PartialViewResult Fields(OrderSubTotalFixedAmount orderSubTotalFixedAmount)
        {
            return PartialView(orderSubTotalFixedAmount);
        }
    }
}