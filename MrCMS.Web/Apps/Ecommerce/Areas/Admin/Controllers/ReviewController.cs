using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;
using NHibernate.Event;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ReviewController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IReviewAdminService _reviewAdminService;
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewAdminService reviewAdminService, IReviewService reviewService)
        {
            _reviewAdminService = reviewAdminService;
            _reviewService = reviewService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.List)]
        public ViewResult Index(ProductReviewSearchQuery searchQuery)
        {
            ViewData["approval-options"] = _reviewAdminService.GetApprovalOptions();
            ViewData["results"] = _reviewAdminService.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Edit)]
        public RedirectToRouteResult Index([IoCModelBinder(typeof(ProductReviewUpdateModelBinder))] ReviewUpdateModel model)
        {
            _reviewAdminService.BulkAction(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Edit)]
        public ViewResult Edit(Review review)
        {
            return View(review);
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Edit)]
        public ActionResult Edit_POST(Review review)
        {
            if (ModelState.IsValid)
            {
                _reviewService.Update(review);

                return RedirectToAction("Index");
            }

            return View(review);
        }

        [HttpGet]
        public PartialViewResult Delete(Review review)
        {
            return PartialView(review);
        }

        [ActionName("Delete")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Delete)]
        public RedirectToRouteResult Delete_POST(Review review)
        {
            _reviewService.Delete(review);
            return RedirectToAction("Index");
        }

        public ViewResult Show(Review review)
        {
            return View(review);
        }

        [HttpPost]
        public RedirectToRouteResult Approval([IoCModelBinder(typeof(ProductReviewApprovalModelBinder))]Review review)
        {
            _reviewService.Update(review);
            return RedirectToAction("Index");
        }
    }
}