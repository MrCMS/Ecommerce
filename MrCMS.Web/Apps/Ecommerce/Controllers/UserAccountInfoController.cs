using System.Threading.Tasks;
using System.Web.Mvc;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountInfoController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IUniquePageService _uniquePageService;
        private readonly IUserManagementService _userService;

        public UserAccountInfoController(IUniquePageService uniquePageService, IUserManagementService userService,
            IAuthorisationService authorisationService)
        {
            _uniquePageService = uniquePageService;
            _userService = userService;
            _authorisationService = authorisationService;
        }

        public ActionResult Show(UserAccountInfo page)
        {
            if (CurrentRequestData.CurrentUser != null)
                return View(page);

            return _uniquePageService.RedirectTo<LoginPage>();
        }

        [HttpGet]
        public ActionResult Form(UserAccountModel model)
        {
            var user = CurrentRequestData.CurrentUser;

            if (user == null)
                return _uniquePageService.RedirectTo<LoginPage>();

            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            ModelState.Clear();

            return View(model);
        }

        [HttpPost]
        public async Task<RedirectResult> UpdateUserInfo(UserAccountModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var user = CurrentRequestData.CurrentUser;
                if (user != null && user.IsActive)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;

                    _userService.SaveUser(user);
                    await _authorisationService.SetAuthCookie(user, false);

                    TempData.SuccessMessages().Add("User Info Updated");
                    return _uniquePageService.RedirectTo<UserAccountInfo>();
                }
            }
            return _uniquePageService.RedirectTo<LoginPage>();
        }
    }
}