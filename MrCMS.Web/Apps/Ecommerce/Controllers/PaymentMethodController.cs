using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentMethodController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cartModel;
        private readonly IOrderService _orderService;

        public PaymentMethodController(CartModel cartModel, IOrderService orderService)
        {
            _cartModel = cartModel;
            _orderService = orderService;
        }

        [HttpGet]
        public PartialViewResult CashOnDelivery()
        {
            return PartialView();
        }

        [HttpPost]
        public RedirectResult CashOnDelivery_POST()
        {
            var order = _orderService.PlaceOrder(_cartModel);
            return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new {id = order.Guid}));
        }
    }
}