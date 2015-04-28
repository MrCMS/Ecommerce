using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.UserAccount;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountOrdersController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IGetUserOrders _getUserOrders;

        public UserAccountOrdersController(IUniquePageService uniquePageService, IGetUserOrders getUserOrders)
        {
            _uniquePageService = uniquePageService;
            _getUserOrders = getUserOrders;
        }

        public ActionResult Show(UserAccountOrders page, [IoCModelBinder(typeof(UserAccountOrdersModelBinder))] UserAccountOrdersSearchModel model)
        {
            // check if logged in
            User user = CurrentRequestData.CurrentUser;
            if (user == null)
                return _uniquePageService.RedirectTo<LoginPage>();

            // Get Orders
            ViewData["orders"] = _getUserOrders.Get(user, model.Page);

            return View(page);
        }
    }
}