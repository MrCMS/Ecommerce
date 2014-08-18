using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class EmptyBasketController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IEmptyBasket _emptyBasket;

        public EmptyBasketController(IEmptyBasket emptyBasket)
        {
            _emptyBasket = emptyBasket;
        }

        [HttpPost]
        public JsonResult Empty()
        {
            _emptyBasket.Empty();
            return Json(true);
        }
    }
}