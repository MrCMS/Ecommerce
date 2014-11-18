using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartRewardPointsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICartRewardPointsUIService _cartRewardPointsUIService;

        public CartRewardPointsController(ICartRewardPointsUIService cartRewardPointsUIService)
        {
            _cartRewardPointsUIService = cartRewardPointsUIService;
        }

        [HttpPost]
        public JsonResult Save(bool useRewardPoints)
        {
            _cartRewardPointsUIService.SetUseRewardPoints(useRewardPoints);
            return Json(true);
        }
    }
}