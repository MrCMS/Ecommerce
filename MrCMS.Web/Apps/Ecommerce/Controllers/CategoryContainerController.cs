using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductContainerController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;

        public ProductContainerController(IUniquePageService uniquePageService)
        {
            _uniquePageService = uniquePageService;
        }

        public RedirectResult Show(ProductContainer page)
        {
            return _uniquePageService.RedirectTo<ProductSearch>();
        }
    }
}