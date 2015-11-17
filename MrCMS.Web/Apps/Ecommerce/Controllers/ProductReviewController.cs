using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductReviewController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductReviewUIService _productReviewUIService;
        private readonly IHelpfulnessVoteService _helpfulnessVoteService;
        private readonly IGetCurrentUser _getCurrentUser;

        public ProductReviewController(IProductReviewUIService productReviewUIService, IHelpfulnessVoteService helpfulnessVoteService, IGetCurrentUser getCurrentUser)
        {
            _productReviewUIService = productReviewUIService;
            _helpfulnessVoteService = helpfulnessVoteService;
            _getCurrentUser = getCurrentUser;
        }

        public PartialViewResult Add(ProductVariant productVariant)
        {
            var user = _getCurrentUser.Get();
            var model = new ProductReview
            {
                ProductVariant = productVariant
            };

            if (user != null)
            {
                model.Name = user.Name;
                model.Email = user.Email;
            }
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Add")]
        [AntiForgeryValidation]
        [HoneypotFilter]
        public RedirectResult Add_POST(ProductReview productReview)
        {
            _productReviewUIService.Add(productReview);

            TempData["review-submitted"] = true;
            
            return Redirect(Referrer.ToString());
        }

        public ActionResult HelpfulnessVotes(ProductReview productReview)
        {
            return PartialView(productReview);
        }

        [HttpPost]
        [ActionName("HelpfulnessVotes")]
        public ActionResult HelpfulnessVotes_POST([IoCModelBinder(typeof(SetIPAddressModelBinder))]HelpfulnessVoteModel voteModel)
        {
            var response = _helpfulnessVoteService.Upvote(voteModel);
            if (Request.IsAjaxRequest())
            {
                return Json(response.IsSuccess());
            }

            TempData["productreview-response-info"] = response;
            return
                Redirect(string.IsNullOrWhiteSpace(response.RedirectUrl)
                    ? Referrer != null
                        ? Referrer.ToString()
                        : "~/"
                    : response.RedirectUrl);

            //if (CurrentRequestData.CurrentUser != null)
            //    vote.User = CurrentRequestData.CurrentUser;

            //vote.IsHelpful = true;

            //_helpfulnessVoteService.Add(vote);

            //TempData["vote-submitted"] = true;

            //return Redirect(Referrer.ToString());
        }

        [HttpPost]
        [ActionName("UnhelpfulnessVotes")]
        public ActionResult UnhelpfulnessVotes([IoCModelBinder(typeof(SetIPAddressModelBinder))]HelpfulnessVoteModel voteModel)
        {
            var response = _helpfulnessVoteService.Downvote(voteModel);
            if (Request.IsAjaxRequest())
            {
                return Json(response.IsSuccess());
            }

            TempData["productreview-response-info"] = response;
            return
                Redirect(string.IsNullOrWhiteSpace(response.RedirectUrl)
                    ? Referrer != null
                        ? Referrer.ToString()
                        : "~/"
                    : response.RedirectUrl);

            //if (CurrentRequestData.CurrentUser != null)
            //    vote.User = CurrentRequestData.CurrentUser;

            //vote.IsHelpful = false;

            //_helpfulnessVoteService.Add(vote);

            //TempData["vote-submitted"] = true;

            //return Redirect(Referrer.ToString());
        }
    }
}