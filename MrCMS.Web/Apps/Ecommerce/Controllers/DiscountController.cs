using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class DiscountController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICartDiscountService _cartDiscountService;
        private const string DiscountResult = "discount-result";

        public DiscountController(ICartDiscountService cartDiscountService)
        {
            _cartDiscountService = cartDiscountService;
        }

        [ChildActionOnly]
        public ActionResult Error()
        {
            var error = TempData[DiscountResult] as CheckCodeResult;
            if (error != null && !error.Success)
            {
                return PartialView(error);
            }
            return new EmptyResult();
        }

        [HttpPost]
        public JsonResult Apply(string discountCode)
        {
            var result = _cartDiscountService.AddDiscountCode(discountCode, false);
            if (!result.Success)
            {
                TempData[DiscountResult] = result;
            }
            return Json(discountCode);
        }

        [HttpPost]
        public JsonResult Remove(string discountCode)
        {
            _cartDiscountService.RemoveDiscountCode(discountCode);
            return Json(discountCode);
        }

        [HttpGet]
        public void Code(string discountCode)
        {
            var result = _cartDiscountService.AddDiscountCode(discountCode, true);

            TempData[DiscountResult] = result;

            Response.Redirect(result.RedirectUrl);
        }
    }
}