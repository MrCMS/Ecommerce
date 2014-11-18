using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public class HelpfulnessVoteService : IHelpfulnessVoteService
    {
        private readonly ISession _session;

        public HelpfulnessVoteService(ISession session)
        {
            _session = session;
        }

        public HelpfulnessVote GetById(int id)
        {
            return _session.QueryOver<HelpfulnessVote>().Where(x => x.Id == id).SingleOrDefault();
        }

        public IList<HelpfulnessVote> GetAll()
        {
            return _session.QueryOver<HelpfulnessVote>().OrderBy(x => x.CreatedOn).Asc.Cacheable().List();
        }

        public void Add(HelpfulnessVote helpfulnessVote)
        {
            _session.Transact(session => session.Save(helpfulnessVote));
        }

        public void Update(HelpfulnessVote helpfulnessVote)
        {
            _session.Transact(session => session.Update(helpfulnessVote));
        }

        public void Delete(HelpfulnessVote helpfulnessVote)
        {
            _session.Transact(session => session.Delete(helpfulnessVote));
        }

        public int GetAllHelpfulVotesCount(Review review)
        {
            return _session.QueryOver<HelpfulnessVote>().Where(a => a.IsHelpful && a.Review == review).List().Count;
        }

        public int GetAllUnhelpfulVotesCount(Review review)
        {
            return _session.QueryOver<HelpfulnessVote>().Where(a => !a.IsHelpful && a.Review == review).List().Count;
        }
    }
}