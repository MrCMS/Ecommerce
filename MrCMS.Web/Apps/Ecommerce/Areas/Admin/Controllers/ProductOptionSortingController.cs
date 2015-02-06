using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductOptionSortingController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductOptionSortingAdminService _productOptionSortingAdminService;

        public ProductOptionSortingController(IProductOptionSortingAdminService productOptionSortingAdminService)
        {
            _productOptionSortingAdminService = productOptionSortingAdminService;
        }

        [HttpGet]
        public ViewResult Index(ProductOptionSortingSearchQuery searchQuery)
        {
            ViewData["results"] = _productOptionSortingAdminService.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        public ViewResult Sort(ProductOption option)
        {
            ViewData["options"] = _productOptionSortingAdminService.GetOptions(option);
            return View(option);
        }

        [HttpPost]
        public RedirectToRouteResult Sort(ProductOption option, [IoCModelBinder(typeof(ProductOptionValueSortingDataModelBinder))]List<ProductOptionValueSortingData> sortingInfo)
        {
            _productOptionSortingAdminService.SaveSorting(option, sortingInfo);
            TempData.SuccessMessages().Add("Sorting saved");
            return RedirectToAction("Sort", "ProductOptionSorting", new {id = option.Id});
        }
    }

    public class ProductOptionValueSortingDataModelBinder : MrCMSDefaultModelBinder
    {
        public ProductOptionValueSortingDataModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var form = controllerContext.HttpContext.Request.Form;
            var valueKeys = form.AllKeys.Where(s => s.StartsWith("value-")).ToList();

            return (from valueKey in valueKeys
                let id = valueKey.Split('-')[1]
                select new ProductOptionValueSortingData
                {
                    DisplayOrder = int.Parse(form["order-" + id]),
                    Value = form[valueKey]
                }).ToList();
        }
    }
}