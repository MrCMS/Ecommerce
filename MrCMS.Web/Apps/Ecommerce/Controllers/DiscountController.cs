using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
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
            var error = TempData[DiscountInvalid] as CheckCodeResult;
            if (error != null && !error.Success)
            {
                return PartialView(error);
            }
            return new EmptyResult();
        }

        [HttpPost]
        public JsonResult Apply(string discountCode)
        {
            var result = _cartDiscountService.AddDiscountCode(discountCode);
            if (!result.Success)
            {
                TempData[DiscountInvalid] = result;
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