using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductVariantController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductVariantAdminService _productVariantAdminService;
        private readonly IReviewService _reviewService;

        public ProductVariantController(IProductVariantAdminService productVariantAdminService, IReviewService reviewService)
        {
            _productVariantAdminService = productVariantAdminService;
            _reviewService = reviewService;
        }

        [HttpGet]
        public PartialViewResult Add(Product product)
        {
            var productVariant = new ProductVariant
            {
                Product = product,
                OptionValues = Enumerable.Range(0, product.Options.Count).Select(i => new ProductOptionValue()).ToList()
            };
            _productVariantAdminService.SetViewData(ViewData, productVariant);
            return PartialView(productVariant);
        }

        [ActionName("Add")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Add_POST([IoCModelBinder(typeof (ProductVariantModelBinder))] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _productVariantAdminService.Add(productVariant);
                return RedirectToAction("Edit", "Webpage", new {id = productVariant.Product.Id});
            }
            return RedirectToAction("Add", "ProductVariant", new {id = productVariant.Product.Id});
        }

        [HttpGet]
        public PartialViewResult Edit(ProductVariant productVariant)
        {
            ModelState.Clear();
            _productVariantAdminService.SetViewData(ViewData, productVariant);
            return PartialView(productVariant);
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Edit_POST([IoCModelBinder(typeof (ProductVariantModelBinder))] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _productVariantAdminService.Update(productVariant);
                return RedirectToAction("Edit", "Webpage", new {id = productVariant.Product.Id});
            }
            return RedirectToAction("Edit", "ProductVariant", new {id = productVariant.Product.Id});
        }

        [HttpGet]
        public PartialViewResult Delete(ProductVariant productVariant)
        {
            return PartialView(productVariant);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(ProductVariant productVariant)
        {
            _productVariantAdminService.Delete(productVariant);
            return RedirectToAction("Edit", "Webpage", new {id = productVariant.Product.Id});
        }

        public JsonResult IsUniqueSKU(string sku, int id = 0)
        {
            return _productVariantAdminService.AnyExistingProductVariantWithSKU(sku, id)
                ? Json("There is already an SKU stored with that value.", JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }

        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.List)]
        public ActionResult ProductReviews(ProductVariant productVariant, int reviewPage = 1, string q = "")
        {
            var reviewsPageSize = MrCMSApplication.Get<ProductReviewSettings>().PageSize;
            ViewData["reviews"] = _reviewService.GetReviewsByProductVariantId(productVariant, reviewPage, reviewsPageSize);

            return PartialView(productVariant);
        }
    }
}