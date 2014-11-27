using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public class ReviewService : IReviewService
    {
        private readonly ISession _session;

        public ReviewService(ISession session)
        {
            _session = session;
        }

        public Review GetById(int id)
        {
            return _session.QueryOver<Review>().Where(x => x.Id == id).SingleOrDefault();
        }

        public IList<Review> GetAll()
        {
            return _session.QueryOver<Review>().OrderBy(x => x.CreatedOn).Desc.Cacheable().List();
        }

        public void Add(Review review)
        {
            _session.Transact(session => session.Save(review));
        }

        public void Update(Review review)
        {
            _session.Transact(session => session.Update(review));
        }

        public void Delete(Review review)
        {
            _session.Transact(session => session.Delete(review));
        }

        public List<SelectListItem> GetRatingOptions()
        {
            return Enumerable.Range(1, 5)
                .BuildSelectItemList(i => i.ToString(), emptyItemText: "Please select");
        }

        public IPagedList<Review> GetReviewsByProductVariantId(ProductVariant productVariant, int pageNum, int pageSize = 10)
        {
            //HelpfulnessVote helpfulnessVoteAlias = null;

            //return _session.QueryOver<Review>()
            //    .JoinAlias(review => review, () => helpfulnessVoteAlias.Review)
            //    .Where(review => review.ProductVariant == productVariant)
            //    .OrderBy(() => helpfulnessVoteAlias.IsHelpful).Desc
            //    .ThenBy(review => review.CreatedOn).Asc.Paged(pageNum, pageSize);

            Review reviewAlias = null;
            return
                GetBaseProductVariantReviewsQuery(_session.QueryOver(() => reviewAlias), productVariant)
                    //.OrderBy(a => a.CreatedOn).Desc
                    .OrderBy(
                        Projections.SubQuery(
                            QueryOver.Of<HelpfulnessVote>()
                                .Where(vote => vote.Review.Id == reviewAlias.Id && vote.IsHelpful)
                                .Select(Projections.Count<HelpfulnessVote>(x => x.Id)))).Desc
                    .Paged(pageNum, pageSize);
        }

        //Ask gary for averaging
        public decimal GetAverageRatingsByProductVariant(ProductVariant productVariant)
        {
            if (!GetBaseProductVariantReviewsQuery(_session.QueryOver<Review>(), productVariant).Cacheable().Any()) 
                return decimal.Zero;
            return GetBaseProductVariantReviewsQuery(_session.QueryOver<Review>(),productVariant)
                .Select(x => x.Rating).Cacheable()
                .List<int>().Select(Convert.ToDecimal).Average();
        }

        private IQueryOver<Review, Review> GetBaseProductVariantReviewsQuery(IQueryOver<Review,Review> query, ProductVariant productVariant)
        {
            return query.Where(review => review.ProductVariant.Id == productVariant.Id && review.Approved == true);
        }

        public IPagedList<Review> GetReviewsByUser(User user, int pageNum, int pageSize = 10)
        {
            int id = user.Id;
            return _session.QueryOver<Review>()
                .Where(x => x.User.Id == id)
                .OrderBy(x => x.CreatedOn)
                .Desc.Paged(pageNum, pageSize);
        }

        public IPagedList<Review> GetPaged(int pageNum, string search, int pageSize = 10)
        {
            return BaseQuery(search).Paged(pageNum, pageSize);
        }

        private IQueryOver<Review, Review> BaseQuery(string search)
        {
            ProductVariant productVariantAlias = null;
            Review reviewAlias = null;

            if (String.IsNullOrWhiteSpace(search))
                return
                _session.QueryOver<Review>()
                        .OrderBy(entry => entry.CreatedOn).Desc;
            return _session.QueryOver(() => reviewAlias)
                .JoinAlias(() => reviewAlias.ProductVariant, () => productVariantAlias)
                .Where(() => productVariantAlias.Name.IsInsensitiveLike(search, MatchMode.Anywhere))
                .OrderBy(entry => entry.CreatedOn).Desc;
        }
    }
}