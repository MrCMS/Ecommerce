using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CategoryContainerController : MrCMSAppUIController<EcommerceApp>
    {
        public ViewResult Show(CategoryContainer page)
        {
            return View(page);
        }
    }
}