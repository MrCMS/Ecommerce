using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

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

            return
                _session.QueryOver<Review>()
                    .Where(review => review.ProductVariant == productVariant && review.Approved == true)
                    .OrderBy(a => a.CreatedOn)
                    .Desc.Paged(pageNum, pageSize);
        }

        public IList<Review> GetReviewsByProductVariantId(ProductVariant productVariant)
        {
            return
                _session.QueryOver<Review>()
                    .Where(review => review.ProductVariant == productVariant)
                    .OrderBy(a => a.CreatedOn)
                    .Asc.Cacheable().List();
        }

        public decimal GetAverageRatingsByProductVariant(ProductVariant productVariant)
        {
            var allRatings = GetReviewsByProductVariantId(productVariant);

            if (allRatings.Any())
            {
                var totalRatings = allRatings.Aggregate(decimal.Zero, (current, rating) => current + rating.Rating);
                var totalRatingsCount = GetReviewsByProductVariantId(productVariant).Count;

                return totalRatings / totalRatingsCount;
            }

            return decimal.Zero;
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

        public void UpdateReviews(List<Review> model)
        {
            foreach (var review in model)
            {
                _session.Transact(session => session.Update(review));
            }
        }

        public IPagedList<Review> Search(ProductReviewSearchQuery query)
        {
            var queryOver = _session.QueryOver<Review>();

            switch (query.ApprovalStatus)
            {
                case ApprovalStatus.Pending:
                    queryOver = queryOver.Where(review => review.Approved == null);
                    break;
                case ApprovalStatus.Rejected:
                    queryOver = queryOver.Where(review => review.Approved == false);
                    break;
                case ApprovalStatus.Approved:
                    queryOver = queryOver.Where(review => review.Approved == true);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(query.ProductName))
            {
                ProductVariant productVariantAlias = null;

                queryOver = queryOver.JoinAlias(review => review.ProductVariant, () => productVariantAlias)
                    .Where(() => productVariantAlias.Name.IsInsensitiveLike(query.ProductName, MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(query.Email))
                queryOver = queryOver.Where(review => review.Email.IsLike(query.Email, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(query.Title))
                queryOver = queryOver.Where(review => review.Title.IsLike(query.Title, MatchMode.Anywhere));
            if (query.DateFrom.HasValue)
                queryOver = queryOver.Where(review => review.CreatedOn >= query.DateFrom);
            if (query.DateTo.HasValue)
                queryOver = queryOver.Where(review => review.CreatedOn < query.DateTo);

            return queryOver.OrderBy(review => review.CreatedOn).Asc.Paged(query.Page);
        }

        public List<SelectListItem> GetApprovalOptions()
        {
            return Enum.GetValues(typeof(ApprovalStatus))
                .Cast<ApprovalStatus>()
                .BuildSelectItemList(status => status.ToString(), emptyItem: null);
        }
    }
}