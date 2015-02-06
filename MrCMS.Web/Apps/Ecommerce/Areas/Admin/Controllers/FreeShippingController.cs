using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class FreeShippingController : MrCMSAppAdminController<EcommerceApp>
    {
        public EmptyResult Fields()
        {
            return new EmptyResult();
        }
    }
}