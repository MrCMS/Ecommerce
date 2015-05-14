using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.UserAccount;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountAddressesController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IGetUserAddresses _getUserAddresses;
        private readonly IExistingAddressService _existingAddressService;

        public UserAccountAddressesController(IUniquePageService uniquePageService, 
            IGetUserAddresses getUserAddresses, IExistingAddressService existingAddressService)
        {
            _uniquePageService = uniquePageService;
            _getUserAddresses = getUserAddresses;
            _existingAddressService = existingAddressService;
        }

        public ActionResult Show(UserAccountAddresses page)
        {
            var user = CurrentRequestData.CurrentUser;
            if (user == null)
                _uniquePageService.RedirectTo<LoginPage>();

            ViewData["addresses"] = _getUserAddresses.Get(user);

            return View(page);
        }

        [HttpPost]
        public RedirectResult DeleteAddress(Address address)
        {
            User user = CurrentRequestData.CurrentUser;
            if (user == null)
                return Redirect("~/");

            if (address.User.Id != user.Id)
                return Redirect("~/");

            _existingAddressService.Delete(address);
            TempData.ErrorMessages().Add("Address deleted.");
            return _uniquePageService.RedirectTo<UserAccountAddresses>();
        }

    }
}