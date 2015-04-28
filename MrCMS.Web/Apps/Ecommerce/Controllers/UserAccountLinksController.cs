using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.UserAccount;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountLinksController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetUserAccountLinks _getUserAccountLinks;

        public UserAccountLinksController(IGetUserAccountLinks getUserAccountLinks)
        {
            _getUserAccountLinks = getUserAccountLinks;
        }

        public PartialViewResult Show()
        {
            return PartialView(_getUserAccountLinks.Get());
        }
    }
}