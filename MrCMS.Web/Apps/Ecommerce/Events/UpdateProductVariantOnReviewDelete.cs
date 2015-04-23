using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Services;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class UpdateProductVariantOnReviewDelete : IOnDeleted<ProductReview>
    {
        private readonly IUpdateProductVariantReviewData _updateProductVariantReviewData;


        public UpdateProductVariantOnReviewDelete(IUpdateProductVariantReviewData updateProductVariantReviewData)
        {
            _updateProductVariantReviewData = updateProductVariantReviewData;
        }

        public void Execute(OnDeletedArgs<ProductReview> args)
        {
            _updateProductVariantReviewData.Update(args.Item.ProductVariant);
        }
    }
}