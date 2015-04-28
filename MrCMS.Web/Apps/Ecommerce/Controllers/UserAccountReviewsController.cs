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
    public class UserAccountReviewsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IGetUserProductReviews _getUserProductReviews;

        public UserAccountReviewsController(IUniquePageService uniquePageService, IGetUserProductReviews getUserProductReviews)
        {
            _uniquePageService = uniquePageService;
            _getUserProductReviews = getUserProductReviews;
        }

        public ActionResult Show(UserAccountReviews page, 
            [IoCModelBinder(typeof(UserAccountReviewsModelBinder))] UserAccountReviewsSearchModel model)
        {
            User user = CurrentRequestData.CurrentUser;
            if (user == null)
                _uniquePageService.RedirectTo<LoginPage>();

            // Get Data
            ViewData["reviews"] = _getUserProductReviews.Get(user, model.Page);

            return View(page);
        }
    }
}