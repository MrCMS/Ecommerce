using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class UserRewardPointsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IUserRewardPointsAdminService _userRewardPointsAdminService;

        public UserRewardPointsController(IUserRewardPointsAdminService userRewardPointsAdminService)
        {
            _userRewardPointsAdminService = userRewardPointsAdminService;
        }

        [ChildActionOnly]
        public PartialViewResult List(User user)
        {
            ViewData["user"] = user;
            return PartialView(_userRewardPointsAdminService.GetAll(user));
        }

        [HttpGet]
        public ActionResult Add(User user)
        {
            return PartialView(_userRewardPointsAdminService.GetDefaultAdjustment(user));
        }

        [HttpPost]
        public ActionResult Add(ManualAdjustment adjustment)
        {
            _userRewardPointsAdminService.AddAdjustment(adjustment);
            return RedirectToAction("Edit", "User", new {id = adjustment.User.Id});
        }
    }
}