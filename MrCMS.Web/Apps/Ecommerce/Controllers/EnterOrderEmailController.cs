using System.Linq;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class EnterOrderEmailController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly ICartManager _cartManager;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IAuthorisationService _authorisationService;
        private readonly IUserService _userService;

        public EnterOrderEmailController(CartModel cart, ICartManager cartManager, IPasswordManagementService passwordManagementService, IAuthorisationService authorisationService, IUserService userService)
        {
            _cart = cart;
            _cartManager = cartManager;
            _passwordManagementService = passwordManagementService;
            _authorisationService = authorisationService;
            _userService = userService;
        }

        public ActionResult Show(EnterOrderEmail page)
        {
            if (CurrentRequestData.CurrentUser == null)
            {
                if (!_cart.Items.Any())
                    return Redirect(UniquePageHelper.GetUrl<Cart>());
                _cart.OrderEmail = _cart.OrderEmail ?? (CurrentRequestData.CurrentUser != null
                                                            ? CurrentRequestData.CurrentUser.Email
                                                            : null);
                ViewData["cart"] = _cart;
                return View(page);
            }
            _cartManager.SetOrderEmail(CurrentRequestData.CurrentUser.Email);
            return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
        }

        [HttpPost]
        public ActionResult SetOrderEmail(string orderEmail)
        {
            _cartManager.SetOrderEmail(orderEmail);
            return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
        }

        [HttpPost]
        public ActionResult SetOrderEmailAndLogin(EmailAndLoginModel model)
        {
            if (model.HavePassword)
            {
                //TODO: replace with ILoginService from 0.3.1 core
                var user = _userService.GetUserByEmail(model.OrderEmail);
                if (user != null)
                {
                    if (_passwordManagementService.ValidateUser(user, model.Password))
                    {
                        _authorisationService.SetAuthCookie(user.Email, false);
                        _cartManager.SetOrderEmail(model.OrderEmail);
                        return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
                    }
                }
                TempData.ErrorMessages().Add("There was an error logging in with the provided email and password");
                return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
            }
            _cartManager.SetOrderEmail(model.OrderEmail);
            return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
        }
    }
}