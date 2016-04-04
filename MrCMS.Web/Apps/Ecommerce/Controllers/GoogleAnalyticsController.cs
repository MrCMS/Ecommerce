using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class GoogleAnalyticsController : MrCMSAppUIController<EcommerceApp>
    {
        public PartialViewResult GetEcommerceTrackingCode()
        {
            if (CurrentRequestData.CurrentPage is OrderPlaced)
            {
                var order = TempData["Order"] as Order;
                if (order != null && order.CreatedOn > CurrentRequestData.Now.AddSeconds(-15))
                {
                    return PartialView("AnalyticsCode", order);
                }
            }

            return null;
        }
    }
}
