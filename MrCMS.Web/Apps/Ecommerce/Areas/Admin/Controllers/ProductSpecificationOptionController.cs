using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductSpecificationOptionController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductOptionManager _productOptionManager;

        public ProductSpecificationOptionController(IProductOptionManager productOptionManager)
        {
            _productOptionManager = productOptionManager;
        }

        public ViewResult Index()
        {
            var options = _productOptionManager.ListSpecificationOptions();
            return View(options);
        }

        [HttpPost]
        public RedirectToRouteResult Add(ProductSpecificationOption option)
        {
            _productOptionManager.AddSpecificationOption(option);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(ProductSpecificationOption option)
        {
            return PartialView(option);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(ProductSpecificationOption option)
        {
            _productOptionManager.UpdateSpecificationOption(option);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(ProductSpecificationOption option)
        {
            return PartialView(option);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(ProductSpecificationOption option)
        {
            _productOptionManager.DeleteSpecificationOption(option);
            return RedirectToAction("Index");
        }
    }
}