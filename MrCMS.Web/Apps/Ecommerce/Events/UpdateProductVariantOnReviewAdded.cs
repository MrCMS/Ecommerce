using System;
using System.Linq;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class UpdateProductVariantOnReviewAdded : IOnAdded<ProductReview>
    {
        private readonly ISession _session;

        public UpdateProductVariantOnReviewAdded(ISession session)
        {
            _session = session;
        }

        public void Execute(OnAddedArgs<ProductReview> args)
        {
            if (args.Item.Approved == false)
                return;

            var productVariant = args.Item.ProductVariant;
            var reviews =
                _session.QueryOver<ProductReview>()
                    .Where(x => x.ProductVariant.Id == productVariant.Id && x.Approved == true)
                    .Cacheable()
                    .List();

            if(!reviews.Any())
                return;

            productVariant.NumberOfReviews = reviews.Count;
            var rating = (decimal)(reviews.Average(x => x.Rating));
            productVariant.Rating = (Math.Round((rating * 2), MidpointRounding.AwayFromZero) / 2);

            _session.Transact(session => session.Update(productVariant));
        }
    }
}