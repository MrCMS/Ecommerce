using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

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
        [ForceImmediateLuceneUpdate]
        public JsonResult Add(ProductOption productOption)
        {
            var option = _productOptionManagementService.Add(productOption);
            return Json(option.Id);
        }
    }
}