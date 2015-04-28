using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public class GetUserProductReviews : IGetUserProductReviews
    {
        private readonly ISession _session;

        public GetUserProductReviews(ISession session)
        {
            _session = session;
        }

        public IPagedList<ProductReview> Get(User user, int page = 1)
        {
            return
                _session.QueryOver<ProductReview>()
                    .Where(review => review.User.Id == user.Id)
                    .OrderBy(review => review.CreatedOn)
                    .Desc
                    .Cacheable()
                    .List()
                    .ToPagedList(page);
        }
    }
}