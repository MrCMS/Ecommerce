using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductVariantController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IOptionService _optionService;
        private readonly IShippingMethodManager _shippingMethodManager;

        public ProductVariantController(IProductVariantService productVariantService, ITaxRateManager taxRateManager,
            IOptionService optionService, IShippingMethodManager shippingMethodManager)
        {
            _productVariantService = productVariantService;
            _taxRateManager = taxRateManager;
            _optionService = optionService;
            _shippingMethodManager = shippingMethodManager;
        }

        [HttpGet]
        public PartialViewResult Add(Product product)
        {
            ViewData["shipping-methods"] = _shippingMethodManager.GetAll();
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions();
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            return
                PartialView(new ProductVariant
                                {
                                    Product = product,
                                    OptionValues =
                                        Enumerable.Range(0, product.Options.Count)
                                                  .Select(i => new ProductOptionValue()).ToList()
                                });
        }

        [ActionName("Add")]
        [HttpPost]
        public ActionResult Add_POST([IoCModelBinder(typeof(ProductVariantModelBinder))]ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _productVariantService.Add(productVariant);
                return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
            }
            return RedirectToAction("Add", "ProductVariant", new { id = productVariant.Product.Id });
        }

        [HttpGet]
        public PartialViewResult Edit(ProductVariant productVariant)
        {
            ViewData["shipping-methods"] = _shippingMethodManager.GetAll();
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            return PartialView(productVariant);
        }

        [ActionName("Edit")]
        [HttpPost]
        public ActionResult Edit_POST([IoCModelBinder(typeof(ProductVariantModelBinder))]ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _productVariantService.Update(productVariant);
                return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
            }
            return RedirectToAction("Edit", "ProductVariant", new { id = productVariant.Product.Id });
        }

        [HttpGet]
        public PartialViewResult Delete(ProductVariant productVariant)
        {
            return PartialView(productVariant);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(ProductVariant productVariant)
        {
            _productVariantService.Delete(productVariant);
            return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
        }

        public JsonResult IsUniqueSKU(string sku, int Id = 0)
        {
            return _productVariantService.AnyExistingProductVariantWithSKU(sku, Id)
                       ? Json("There is already an SKU stored with that value.", JsonRequestBehavior.AllowGet)
                       : Json(true, JsonRequestBehavior.AllowGet);
        }
    }

    public class ProductVariantModelBinder : MrCMSDefaultModelBinder
    {
        private const string ShippingMethodPrefix = "shipping-method-";

        public ProductVariantModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);
            if (bindModel is ProductVariant)
            {
                var productVariant = bindModel as ProductVariant;

                var methodKeys =
                    controllerContext.HttpContext.Request.Params.AllKeys.Where(s => s.StartsWith(ShippingMethodPrefix))
                                     .ToList();

                var excludedMethods = new List<ShippingMethod>();
                foreach (var key in methodKeys)
                {
                    var method = Session.Get<ShippingMethod>(Convert.ToInt32(key.Replace(ShippingMethodPrefix, "")));
                    excludedMethods.Add(method);
                }
                productVariant.RestrictedShippingMethods = excludedMethods;
            }
            return bindModel;
        }
    }
}