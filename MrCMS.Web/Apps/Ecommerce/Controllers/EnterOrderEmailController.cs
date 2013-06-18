using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class EnterOrderEmailController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;

        public EnterOrderEmailController(IGetCart getCart)
        {
            _getCart = getCart;
        }

        public ViewResult Show(EnterOrderEmail page)
        {
            return View(page);
        }
    }
}