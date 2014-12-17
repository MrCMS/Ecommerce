using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class DiscountController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICartDiscountService _cartDiscountService;
        private const string DiscountInvalid = "discount-invalid";

        public DiscountController(ICartDiscountService cartDiscountService)
        {
            _cartDiscountService = cartDiscountService;
        }

        [ChildActionOnly]
        public ActionResult Error()
        {
            if (ShowError())
            {
                return PartialView();
            }
            return new EmptyResult();
        }

        private bool ShowError()
        {
            var o = TempData[DiscountInvalid];
            return o is bool && (bool) o;
        }

        [HttpPost]
        public JsonResult Apply(string discountCode)
        {
            var success = _cartDiscountService.AddDiscountCode(discountCode);
            if (!success)
            {
                TempData[DiscountInvalid] = true;
            }
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