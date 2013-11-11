using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SagePayController : MrCMSAppUIController<EcommerceApp>
    {
        public SagePayController()
        {
            
        }

        public ActionResult Failed(string vendortxcode)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult Success(string vendortxcode)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult Notification()
        {
            throw new System.NotImplementedException();
        }
    }
}