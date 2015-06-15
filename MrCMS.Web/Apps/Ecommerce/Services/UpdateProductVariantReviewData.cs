using System;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class UpdateProductVariantReviewData : IUpdateProductVariantReviewData
    {
        private readonly ISession _session;

        public UpdateProductVariantReviewData(ISession session)
        {
            _session = session;
        }

        public void Update(ProductVariant productVariant)
        {
            var count =
                GetBaseQuery(productVariant).Cacheable().RowCount();

            var total =
                GetBaseQuery(productVariant)
                    .Select(Projections.Sum<ProductReview>(x => x.Rating))
                    .Cacheable()
                    .SingleOrDefault<int>();

            if (count == 0)
            {
                productVariant.NumberOfReviews = 0;
                productVariant.Rating = 0;
            }
            else
            {
                productVariant.NumberOfReviews = count;
                var rating = (decimal) total/count;
                productVariant.Rating = (Math.Round((rating * 2), MidpointRounding.AwayFromZero) / 2);
            }

            _session.Transact(session => session.Update(productVariant));
        }

        private IQueryOver<ProductReview, ProductReview> GetBaseQuery(ProductVariant productVariant)
        {
            return _session.QueryOver<ProductReview>()
                .Where(x => x.ProductVariant.Id == productVariant.Id && x.Approved == true);
        }
    }
}