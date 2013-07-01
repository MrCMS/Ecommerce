using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserRegistrationController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUserService _userService;
        private readonly IAuthorisationService _authorisationService;

        public UserRegistrationController(IUserService userService, IAuthorisationService authorisationService)
        {
            _userService = userService;
            _authorisationService = authorisationService;
        }

        public ActionResult UserRegistration(UserRegistration page)
        {
            if (CurrentRequestData.CurrentUser != null)
                return Redirect(UniquePageHelper.GetUrl<UserAccount>());
            return View(page);
        }

        [HttpGet]
        public ViewResult UserRegistrationDetails(UserRegistrationModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(UserRegistrationModel model)
        {
            if (CurrentRequestData.CurrentUser != null)
                return Redirect(UniquePageHelper.GetUrl<UserAccount>());
            if (model != null && ModelState.IsValid)
            {
                var user = new User
                    {
                        FirstName = model.FirstName, 
                        LastName = model.LastName, 
                        Email = model.Email,
                        IsActive = true
                    };
                _authorisationService.SetPassword(user,model.Password,model.ConfirmPassword);
                _userService.AddUser(user);
                _authorisationService.SetAuthCookie(model.Email,false);
                return Redirect(UniquePageHelper.GetUrl<UserAccount>());
            }
            return Redirect(UniquePageHelper.GetUrl<UserRegistration>());
        }
    }
}