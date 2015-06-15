using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Filters;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.UserAccount;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountEditAddressController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IGetCountryOptions _getCountryOptions;
        private readonly IExistingAddressService _existingAddressService;

        public UserAccountEditAddressController(IUniquePageService uniquePageService,
            IGetCountryOptions getCountryOptions, IExistingAddressService existingAddressService)
        {
            _uniquePageService = uniquePageService;
            _getCountryOptions = getCountryOptions;
            _existingAddressService = existingAddressService;
        }

        [MustBeLoggedIn]
        public ActionResult Show(UserAccountEditAddress page, [IoCModelBinder(typeof(EditUserAddressModelBinder))] Address address)
        {
            if (address == null)
                return Redirect("~/");

            User user = CurrentRequestData.CurrentUser;

            if (address.User.Id != user.Id)
                return Redirect("~/");

            ViewData["address"] = address;
            ViewData["country-options"] = _getCountryOptions.Get();

            return View(page);
        }

        [HttpPost]
        public RedirectResult Edit(Address address)
        {
            User user = CurrentRequestData.CurrentUser;
            if (address.User.Id != user.Id)
                return Redirect("~/");

            _existingAddressService.Update(address);
            TempData.SuccessMessages().Add("Address updated.");
            return _uniquePageService.RedirectTo<UserAccountAddresses>();
        }
    }
}