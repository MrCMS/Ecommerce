using System.Collections.Generic;
using System.Linq;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using Product = MrCMS.Web.Apps.Ecommerce.Pages.Product;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public class HelpfulnessVoteService : IHelpfulnessVoteService
    {
        private readonly ISession _session;

        public HelpfulnessVoteService(ISession session)
        {
            _session = session;
        }

        public HelpfulnessVoteResponse Upvote(HelpfulnessVoteModel voteModel)
        {
            var productReview = _session.Get<ProductReview>(voteModel.ProductReviewId);
            if (productReview == null)
            {
                return new HelpfulnessVoteResponse()
                {
                    Type = ProductReviewResponseType.Error,
                    Message = "Could not find product review to vote"
                };
            }

            Product productPage = productReview.ProductVariant.Product;
            User currentUser = CurrentRequestData.CurrentUser;
            if (currentUser != null)
            {
                return RegisterLoggedInVote(voteModel, productReview, currentUser, productPage, true);
            }

            return RegisterAnonymousVote(voteModel, productReview, productPage, true);
        }

        public HelpfulnessVoteResponse Downvote(HelpfulnessVoteModel voteModel)
        {
            var productReview = _session.Get<ProductReview>(voteModel.ProductReviewId);
            if (productReview == null)
            {
                return new HelpfulnessVoteResponse
                {
                    Type = ProductReviewResponseType.Error,
                    Message = "Could not find product review to vote"
                };
            }
            Product productPage = productReview.ProductVariant.Product;
            User currentUser = CurrentRequestData.CurrentUser;
            if (currentUser != null)
            {
                return RegisterLoggedInVote(voteModel, productReview, currentUser, productPage, false);
            }
            return RegisterAnonymousVote(voteModel, productReview, productPage, false);
        }

        private HelpfulnessVoteResponse RegisterAnonymousVote(HelpfulnessVoteModel voteModel, ProductReview productReview, Product productPage, bool isHelpful)
        {
            if (productReview.Votes.Any(v => v.IPAddress == voteModel.IPAddress))
            {
                return new HelpfulnessVoteResponse
                {
                    Type = ProductReviewResponseType.Info,
                    Message = "Already voted",
                    RedirectUrl = "~/" + productPage.LiveUrlSegment
                };
            }

            var vote = new HelpfulnessVote
            {
                IsHelpful = isHelpful,
                ProductReview = productReview,
                IPAddress = voteModel.IPAddress
            };

            productReview.Votes.Add(vote);
            _session.Transact(session => session.Save(vote));
            return new HelpfulnessVoteResponse
            {
                Message = "Vote registered",
                RedirectUrl = "~/" + productPage.LiveUrlSegment,
                Type = ProductReviewResponseType.Success
            };
        }

        private HelpfulnessVoteResponse RegisterLoggedInVote(HelpfulnessVoteModel voteModel, ProductReview productReview, User currentUser, Product productPage, bool isHelpful)
        {
            if (productReview.Votes.Any(v => v.IsHelpful == isHelpful && v.User == currentUser))
            {
                return new HelpfulnessVoteResponse
                {
                    Type = ProductReviewResponseType.Info,
                    Message = "Already voted",
                    RedirectUrl = "~/" + productPage.LiveUrlSegment
                };
            }
            List<HelpfulnessVote> oppositeVotes =
                productReview.Votes.Where(v => v.IsHelpful != isHelpful && v.User == currentUser).ToList();
            if (oppositeVotes.Any())
            {
                _session.Transact(session => oppositeVotes.ForEach(v =>
                {
                    productReview.Votes.Remove(v);
                    session.Delete(v);
                }));
            }
            var vote = new HelpfulnessVote
            {
                IsHelpful = isHelpful,
                User = currentUser,
                ProductReview = productReview,
                IPAddress = voteModel.IPAddress
            };
            productReview.Votes.Add(vote);
            _session.Transact(session => session.Save(vote));
            return new HelpfulnessVoteResponse
            {
                Message = "Vote registered",
                RedirectUrl = "~/" + productPage.LiveUrlSegment,
                Type = ProductReviewResponseType.Success
            };
        }
    }
}