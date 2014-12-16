using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductReviewController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductReviewUIService _productReviewUIService;
        private readonly IHelpfulnessVoteService _helpfulnessVoteService;

        public ProductReviewController(IProductReviewUIService productReviewUIService, IHelpfulnessVoteService helpfulnessVoteService)
        {
            _productReviewUIService = productReviewUIService;
            _helpfulnessVoteService = helpfulnessVoteService;
        }

        public PartialViewResult Add(ProductVariant productVariant)
        {
            var model = new ProductReview
            {
                ProductVariant = productVariant
            };
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectResult Add_POST(ProductReview productReview)
        {
            if (CurrentRequestData.CurrentUser != null)
                productReview.User = CurrentRequestData.CurrentUser;

            _productReviewUIService.Add(productReview);

            TempData["review-submitted"] = true;
            
            return Redirect(Referrer.ToString());
        }

        public PartialViewResult HelpfulnessVotes(ProductReview productReview)
        {
            var model = new HelpfulnessVote
            {
                ProductReview = productReview
            };

            ViewData["helpful-count"] = _helpfulnessVoteService.GetAllHelpfulVotesCount(productReview);
            ViewData["unhelpful-count"] = _helpfulnessVoteService.GetAllUnhelpfulVotesCount(productReview);

            return PartialView(model);
        }

        [HttpPost]
        [ActionName("HelpfulnessVotes")]
        public RedirectResult HelpfulnessVotes_POST(HelpfulnessVote vote)
        {
            if (CurrentRequestData.CurrentUser != null)
                vote.User = CurrentRequestData.CurrentUser;

            vote.IsHelpful = true;

            _helpfulnessVoteService.Add(vote);

            TempData["vote-submitted"] = true;

            return Redirect(Referrer.ToString());
        }

        [HttpPost]
        [ActionName("UnhelpfulnessVotes")]
        public RedirectResult UnhelpfulnessVotes(HelpfulnessVote vote)
        {
            if (CurrentRequestData.CurrentUser != null)
                vote.User = CurrentRequestData.CurrentUser;

            vote.IsHelpful = false;

            _helpfulnessVoteService.Add(vote);

            TempData["vote-submitted"] = true;

            return Redirect(Referrer.ToString());
        }
    }
}