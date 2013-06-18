using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class OrderPlacedController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;

        public OrderPlacedController(IGetCart getCart)
        {
            _getCart = getCart;
        }

        public ViewResult Show(OrderPlaced page)
        {
            return View(page);
        }
    }
}