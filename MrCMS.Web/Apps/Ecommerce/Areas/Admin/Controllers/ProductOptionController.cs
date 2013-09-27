using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductOptionController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductOptionManagementService _productOptionManagementService;

        public ProductOptionController(IProductOptionManagementService productOptionManagementService)
        {
            _productOptionManagementService = productOptionManagementService;
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView(new ProductOption());
        }

        [HttpPost]
        public JsonResult Add(ProductOption productOption)
        {
            var option = _productOptionManagementService.Add(productOption);
            return Json(option.Id);
        }
    }
}