using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
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
        private readonly IGetCountryOptions _getCountryOptions;
        private readonly IExistingAddressService _existingAddressService;

        public UserAccountAddressesController(IUniquePageService uniquePageService, 
            IGetUserAddresses getUserAddresses, IGetCountryOptions getCountryOptions, 
            IExistingAddressService existingAddressService)
        {
            _uniquePageService = uniquePageService;
            _getUserAddresses = getUserAddresses;
            _getCountryOptions = getCountryOptions;
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

        [HttpGet]
        public PartialViewResult EditAddress(Address address)
        {
            ViewData["country-options"] = _getCountryOptions.Get();
            return PartialView(address);
        }

        [HttpPost]
        [ActionName("EditAddress")]
        public RedirectResult EditAddress_POST(Address address)
        {
            User user = CurrentRequestData.CurrentUser;
            if (address.User.Id != user.Id)
                return Redirect("~/");

            _existingAddressService.Update(address);
            TempData.SuccessMessages().Add("Address updated.");
            return _uniquePageService.RedirectTo<UserAccountAddresses>();
        }

        [HttpGet]
        public PartialViewResult DeleteAddress(Address address)
        {
            return PartialView(address);
        }

        public RedirectResult DeleteAddress_POST(Address address)
        {
            User user = CurrentRequestData.CurrentUser;
            if (address.User.Id != user.Id)
                return Redirect("~/");

            _existingAddressService.Delete(address);
            TempData.ErrorMessages().Add("Address deleted.");
            return _uniquePageService.RedirectTo<UserAccountAddresses>();
        }

    }
}