using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class DiscountController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICartDiscountService _cartDiscountService;

        public DiscountController(ICartDiscountService cartDiscountService)
        {
            _cartDiscountService = cartDiscountService;
        }

        [HttpPost]
        public JsonResult Apply(string discountCode)
        {
            if (string.IsNullOrWhiteSpace(discountCode))
            {
                _cartDiscountService.SetDiscountCode(discountCode);
                return Json("Removed");
            }
            _cartDiscountService.SetDiscountCode(discountCode);
            return Json(discountCode);
        }
    }
}