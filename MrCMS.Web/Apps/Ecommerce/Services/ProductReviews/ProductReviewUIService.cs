using System;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public class ProductReviewUIService : IProductReviewUIService
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly ISession _session;

        public ProductReviewUIService(ISession session, IGetCurrentUser getCurrentUser)
        {
            _session = session;
            _getCurrentUser = getCurrentUser;
        }

        public void Add(ProductReview productReview)
        {
            User user = _getCurrentUser.Get();
            if (user != null)
            {
                productReview.User = user;
                productReview.Name = user.Name;
                productReview.Email = user.Email;
            }
            _session.Transact(session => session.Save(productReview));
        }

        public void Update(ProductReview productReview)
        {
            _session.Transact(session => session.Update(productReview));
        }

        public void Delete(ProductReview productReview)
        {
            _session.Transact(session => session.Delete(productReview));
        }

        public IPagedList<ProductReview> GetReviewsForVariant(ProductVariant productVariant, int pageNum,
            int pageSize = 10)
        {
            ProductReview productReviewAlias = null;
            return
                GetBaseProductVariantReviewsQuery(_session.QueryOver(() => productReviewAlias), productVariant)
                    .OrderBy(
                        Projections.SubQuery(
                            QueryOver.Of<HelpfulnessVote>()
                                .Where(vote => vote.ProductReview.Id == productReviewAlias.Id && vote.IsHelpful)
                                .Select(Projections.Count<HelpfulnessVote>(x => x.Id)))).Desc
                    .Paged(pageNum, pageSize);
        }

        //Ask gary for averaging
        public decimal GetAverageRatingForProductVariant(ProductVariant productVariant)
        {
            if (
                !GetBaseProductVariantReviewsQuery(_session.QueryOver<ProductReview>(), productVariant)
                    .Cacheable()
                    .Any())
                return decimal.Zero;
            return GetBaseProductVariantReviewsQuery(_session.QueryOver<ProductReview>(), productVariant)
                .Select(x => x.Rating).Cacheable()
                .List<int>().Select(Convert.ToDecimal).Average();
        }

        public IPagedList<ProductReview> GetReviewsByUser(User user, int pageNum, int pageSize = 10)
        {
            int id = user.Id;
            return _session.QueryOver<ProductReview>()
                .Where(x => x.User.Id == id)
                .OrderBy(x => x.CreatedOn)
                .Desc.Paged(pageNum, pageSize);
        }

        private IQueryOver<ProductReview, ProductReview> GetBaseProductVariantReviewsQuery(
            IQueryOver<ProductReview, ProductReview> query, ProductVariant productVariant)
        {
            return query.Where(review => review.ProductVariant.Id == productVariant.Id && review.Approved == true);
        }

        private IQueryOver<ProductReview, ProductReview> BaseQuery(string search)
        {
            ProductVariant productVariantAlias = null;
            ProductReview productReviewAlias = null;

            if (String.IsNullOrWhiteSpace(search))
                return
                    _session.QueryOver<ProductReview>()
                        .OrderBy(entry => entry.CreatedOn).Desc;
            return _session.QueryOver(() => productReviewAlias)
                .JoinAlias(() => productReviewAlias.ProductVariant, () => productVariantAlias)
                .Where(() => productVariantAlias.Name.IsInsensitiveLike(search, MatchMode.Anywhere))
                .OrderBy(entry => entry.CreatedOn).Desc;
        }
    }
}