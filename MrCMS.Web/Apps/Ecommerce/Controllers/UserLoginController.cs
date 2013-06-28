using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Controllers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserLoginController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUserService _userService;
        private readonly IAuthorisationService _authorisationService;

        public UserLoginController(IUserService userService, IAuthorisationService authorisationService)
        {
            _userService = userService;
            _authorisationService = authorisationService;
        }

        public ActionResult UserLogin(UserLogin page, bool logout = false)
        {
            if (logout)
            {
                _authorisationService.Logout();
                return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
            }
            if (CurrentRequestData.CurrentUser != null)
                return Redirect(UniquePageHelper.GetUrl<UserAccount>());
            return View(page);
        }

        [HttpGet]
        public ViewResult UserLoginDetails(LoginController.LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginController.LoginModel model)
        {
            if (model != null)
            {
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null && user.IsActive)
                {
                    if (_authorisationService.ValidateUser(user, model.Password))
                    {
                        _authorisationService.SetAuthCookie(model.Email, model.RememberMe);
                        return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
                    }
                }
            }
            return Redirect(UniquePageHelper.GetUrl<UserLogin>());
        }
    }
}