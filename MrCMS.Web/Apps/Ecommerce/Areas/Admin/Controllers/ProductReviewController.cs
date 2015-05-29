using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductReviewController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductReviewAdminService _productReviewAdminService;

        public ProductReviewController(IProductReviewAdminService productReviewAdminService)
        {
            _productReviewAdminService = productReviewAdminService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.List)]
        public ViewResult Index(ProductReviewSearchQuery searchQuery)
        {
            ViewData["approval-options"] = _productReviewAdminService.GetApprovalOptions();
            ViewData["results"] = _productReviewAdminService.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Edit)]
        public RedirectToRouteResult Index([IoCModelBinder(typeof(ProductReviewUpdateModelBinder))] ReviewUpdateModel model)
        {
            _productReviewAdminService.BulkAction(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Edit)]
        public ViewResult Edit(ProductReview productReview)
        {
            return View(productReview);
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Edit)]
        public ActionResult Edit_POST(ProductReview productReview)
        {
            _productReviewAdminService.Update(productReview);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(ProductReview productReview)
        {
            return PartialView(productReview);
        }

        [ActionName("Delete")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Delete)]
        public RedirectToRouteResult Delete_POST(ProductReview productReview)
        {
            _productReviewAdminService.Delete(productReview);
            return RedirectToAction("Index");
        }

        public ViewResult Show(ProductReview productReview)
        {
            return View(productReview);
        }

        [HttpPost]
        public RedirectToRouteResult Approval(
            [IoCModelBinder(typeof(ProductReviewApprovalModelBinder))] ProductReview productReview)
        {
            _productReviewAdminService.Update(productReview);
            return RedirectToAction("Index");
        }
    }
}