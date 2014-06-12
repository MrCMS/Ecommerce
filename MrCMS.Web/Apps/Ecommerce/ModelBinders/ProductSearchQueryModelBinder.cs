using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class ProductSearchQueryModelBinder : MrCMSDefaultModelBinder
    {
        public ProductSearchQueryModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.IsChildAction)
            {
                return base.BindModel(controllerContext, bindingContext);
            }
            var model = new ProductSearchQuery
                        {
                            Specifications =
                                (controllerContext.HttpContext.Request["Specifications"] ??
                                 string.Empty).Split(new[] { '|' },
                                     StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => Convert.ToInt32((string) s))
                                .ToList(),
                            Options =
                                (controllerContext.HttpContext.Request["Options"] ?? string.Empty)
                                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList(),
                            PageSize = !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["PageSize"])
                                ? Convert.ToInt32(controllerContext.HttpContext.Request["PageSize"])
                                : MrCMSApplication.Get<EcommerceSettings>()
                                .ProductPerPageOptions.FirstOrDefault(),
                            Page = !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["Page"])
                                ? Convert.ToInt32(controllerContext.HttpContext.Request["Page"])
                                : 1,
                            CategoryId =
                                !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["CategoryId"])
                                    ? Convert.ToInt32(controllerContext.HttpContext.Request["CategoryId"])
                                    : (int?)null,
                            PriceFrom =
                                !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["PriceFrom"])
                                    ? Convert.ToDouble(controllerContext.HttpContext.Request["PriceFrom"])
                                    : 0,
                            PriceTo = !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["PriceTo"])
                                ? Convert.ToDouble(controllerContext.HttpContext.Request["PriceTo"])
                                : (double?)null,
                            BrandId =
                                !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["BrandId"])
                                    ? Convert.ToInt32(controllerContext.HttpContext.Request["BrandId"])
                                    : (int?)null,
                            SearchTerm = controllerContext.HttpContext.Request["SearchTerm"]
                        };

            model.SortBy = !string.IsNullOrWhiteSpace(controllerContext.HttpContext.Request["SortBy"])
                ? (ProductSearchSort)Convert.ToInt32(controllerContext.HttpContext.Request["SortBy"])
                : model.SortBy;

            return model;
        }
    }
}