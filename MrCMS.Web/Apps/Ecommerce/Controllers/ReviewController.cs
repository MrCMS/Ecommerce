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
    public class ReviewController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IReviewService _reviewService;
        private readonly IHelpfulnessVoteService _helpfulnessVoteService;

        public ReviewController(IReviewService reviewService, IHelpfulnessVoteService helpfulnessVoteService)
        {
            _reviewService = reviewService;
            _helpfulnessVoteService = helpfulnessVoteService;
        }

        public PartialViewResult Add(ProductVariant productVariant)
        {
            var model = new Review
            {
                ProductVariant = productVariant
            };
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectResult Add_POST(Review review)
        {
            if (CurrentRequestData.CurrentUser != null)
                review.User = CurrentRequestData.CurrentUser;

            _reviewService.Add(review);

            TempData["review-submitted"] = true;
            

            return Redirect(Referrer.ToString());
        }

        public PartialViewResult HelpfulnessVotes(Review review)
        {
            var model = new HelpfulnessVote
            {
                Review = review
            };

            ViewData["helpful-count"] = _helpfulnessVoteService.GetAllHelpfulVotesCount(review);
            ViewData["unhelpful-count"] = _helpfulnessVoteService.GetAllUnhelpfulVotesCount(review);

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