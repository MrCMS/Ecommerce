using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Filters;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models.UserAccount;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.UserAccount;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountReviewsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetUserProductReviews _getUserProductReviews;

        public UserAccountReviewsController(IGetUserProductReviews getUserProductReviews)
        {
            _getUserProductReviews = getUserProductReviews;
        }

        [MustBeLoggedIn]
        public ActionResult Show(UserAccountReviews page, 
            [IoCModelBinder(typeof(UserAccountReviewsModelBinder))] UserAccountReviewsSearchModel model)
        {
            User user = CurrentRequestData.CurrentUser;

            // Get Data
            ViewData["reviews"] = _getUserProductReviews.Get(user, model.Page);

            return View(page);
        }
    }
}