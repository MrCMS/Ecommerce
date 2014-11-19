using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
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
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.List)]
        public ViewResult Index(ProductReviewSearchQuery searchQuery)
        {
            ViewData["approval-options"] = _reviewService.GetApprovalOptions();
            ViewData["results"] = _reviewService.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(ProductReviewACL), ProductReviewACL.Edit)]
        public RedirectToRouteResult Index([IoCModelBinder(typeof(ProductReviewUpdateModelBinder))] List<ReviewUpdateModel> model)
        {
            var currentOperation = model.First().CurrentOperation;

            var reviewsToBeProcessed = model.Where(item => item.Approved).ToList();

            if (currentOperation == "Approve")
            {
                foreach (var item in reviewsToBeProcessed)
                {
                    var review = _reviewService.GetById(item.ReviewId);
                    review.Approved = true;
                    _reviewService.Update(review);
                }
            }
            else if (currentOperation == "Reject")
            {
                foreach (var item in reviewsToBeProcessed)
                {
                    var review = _reviewService.GetById(item.ReviewId);
                    review.Approved = false;
                    _reviewService.Update(review);
                }
            }
            else
            {
                foreach (var item in reviewsToBeProcessed)
                {
                    var review = _reviewService.GetById(item.ReviewId);
                    _reviewService.Delete(review);
                }
            }

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