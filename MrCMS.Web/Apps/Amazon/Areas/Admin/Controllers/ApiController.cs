using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class ApiController : MrCMSAppAdminController<AmazonApp>
    {
        [HttpGet]
        public ViewResult Dashboard()
        {
            return View();
        }
    }
}
