using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public interface IHelpfulnessVoteService
    {
        HelpfulnessVote GetById(int id);

        IList<HelpfulnessVote> GetAll();

        void Add(HelpfulnessVote review);

        void Update(HelpfulnessVote review);

        void Delete(HelpfulnessVote review);
        int GetAllHelpfulVotesCount(ProductReview productReview);

        int GetAllUnhelpfulVotesCount(ProductReview productReview);
    }
}