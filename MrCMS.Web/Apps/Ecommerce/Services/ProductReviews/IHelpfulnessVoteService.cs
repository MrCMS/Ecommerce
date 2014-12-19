using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public interface IHelpfulnessVoteService
    {
        HelpfulnessVoteResponse Upvote(HelpfulnessVoteModel voteModel);
        HelpfulnessVoteResponse Downvote(HelpfulnessVoteModel voteModel);
    }
}