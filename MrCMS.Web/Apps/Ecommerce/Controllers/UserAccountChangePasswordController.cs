using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Models.UserAccount;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.UserAccount;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountChangePasswordController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IUpdatePasswordService _updatePasswordService;

        public UserAccountChangePasswordController(IUniquePageService uniquePageService, IUpdatePasswordService updatePasswordService)
        {
            _uniquePageService = uniquePageService;
            _updatePasswordService = updatePasswordService;
        }

        public ActionResult Show(UserAccountChangePassword page)
        {
            User user = CurrentRequestData.CurrentUser;
            if (user == null)
                return _uniquePageService.RedirectTo<LoginPage>();

            return View(page);
        }

        [HttpGet]
        public ActionResult UpdatePassword(UpdatePasswordModel model)
        {
            ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ActionName("UpdatePassword")]
        public RedirectResult UpdatePassword_POST(UpdatePasswordModel model)
        {
            var result = _updatePasswordService.Update(model);

            if (result.Success)
            {
                TempData.SuccessMessages().Add(result.Message);
            }
            else
            {
                TempData.ErrorMessages().Add(result.Message);
            }

            return _uniquePageService.RedirectTo<UserAccountChangePassword>();
        }
    }
}