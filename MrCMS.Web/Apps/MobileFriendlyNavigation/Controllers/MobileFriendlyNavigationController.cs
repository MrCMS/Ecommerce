using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Controllers
{
    public class MobileFriendlyNavigationController : MrCMSAppUIController<MobileFriendlyNavigationApp>
    {
        private readonly IMobileFriendlyNavigationService _navigationService;

        public MobileFriendlyNavigationController(IMobileFriendlyNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [HttpGet]
        public ActionResult GetChildNodes(Webpage parent)
        {
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = _navigationService.GetChildNodes(parent).Select(x => x.ToJson())
            };
        }
    }
}