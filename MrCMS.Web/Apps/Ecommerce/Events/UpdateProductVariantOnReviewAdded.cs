using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Services;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class UpdateProductVariantOnReviewAdded : IOnAdded<ProductReview>
    {
        private readonly IUpdateProductVariantReviewData _updateProductVariantReviewData;

        public UpdateProductVariantOnReviewAdded(IUpdateProductVariantReviewData updateProductVariantReviewData)
        {
            _updateProductVariantReviewData = updateProductVariantReviewData;
        }

        public void Execute(OnAddedArgs<ProductReview> args)
        {
            _updateProductVariantReviewData.Update(args.Item.ProductVariant);
        }
    }
}