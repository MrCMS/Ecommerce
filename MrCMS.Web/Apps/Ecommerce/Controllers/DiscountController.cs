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
            _cartDiscountService.AddDiscountCode(discountCode);
            return Json(discountCode);
        }
        [HttpPost]
        public JsonResult Remove(string discountCode)
        {
            _cartDiscountService.RemoveDiscountCode(discountCode);
            return Json(discountCode);
        }
    }
}