using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Services;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class UpdateProductVariantOnReviewUpdate : IOnUpdated<ProductReview>
    {
        private readonly IUpdateProductVariantReviewData _updateProductVariantReviewData;

        public UpdateProductVariantOnReviewUpdate(IUpdateProductVariantReviewData updateProductVariantReviewData)
        {
            _updateProductVariantReviewData = updateProductVariantReviewData;
        }

        public void Execute(OnUpdatedArgs<ProductReview> args)
        {
            _updateProductVariantReviewData.Update(args.Item.ProductVariant);
        }
    }
}