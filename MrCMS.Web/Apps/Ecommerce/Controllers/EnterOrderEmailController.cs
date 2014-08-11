using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Web.Apps.Core.Services;
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
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;

        public EnterOrderEmailController(CartModel cart, ICartManager cartManager, ILoginService loginService, IUserService userService)
        {
            _cart = cart;
            _cartManager = cartManager;
            _loginService = loginService;
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
            return Redirect(UniquePageHelper.GetUrl<SetShippingDetails>());
        }

        [HttpPost]
        public ActionResult SetOrderEmail(string orderEmail)
        {
            _cartManager.SetOrderEmail(orderEmail);
            return Redirect(UniquePageHelper.GetUrl<SetShippingDetails>());
        }

        [HttpPost]
        public async Task<RedirectResult> SetOrderEmailAndLogin(EmailAndLoginModel model)
        {
            if (model.HavePassword)
            {
                var user = _userService.GetUserByEmail(model.OrderEmail.Trim());
                if (user != null)
                {
                    var authenticated =await _loginService.AuthenticateUser(new LoginModel
                                                       {
                                                           Email = user.Email,
                                                           Password = model.Password
                                                       });
                    if (authenticated.Success)
                    {
                        return Redirect(UniquePageHelper.GetUrl<SetShippingDetails>());
                    }
                }
                TempData.ErrorMessages().Add("There was an error logging in with the provided email and password");
                return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
            }
            _cartManager.SetOrderEmail(model.OrderEmail);
            return Redirect(UniquePageHelper.GetUrl<SetShippingDetails>());
        }
    }
}