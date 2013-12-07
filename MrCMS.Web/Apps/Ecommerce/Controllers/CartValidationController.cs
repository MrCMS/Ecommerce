using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartValidationController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICartValidationService _cartValidationService;

        public CartValidationController(ICartValidationService cartValidationService)
        {
            _cartValidationService = cartValidationService;
        }
    }
}