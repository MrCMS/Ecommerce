using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class OrderPlacedController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cartModel;
        private readonly IOrderService _orderService;

        public OrderPlacedController(CartModel cartModel, IOrderService orderService)
        {
            _cartModel = cartModel;
            _orderService = orderService;
        }

        public ActionResult Show(OrderPlaced page, int oid = 0)
        {
            if (_orderService.Get(oid) != null && !string.IsNullOrWhiteSpace(_cartModel.OrderEmail))
            {
                ViewBag.OrderID = oid;
                ViewBag.OrderEmail = _cartModel.OrderEmail;
                return View(page);
            }
            return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
        }
    }
}