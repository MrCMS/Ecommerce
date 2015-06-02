using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class ProductReviewResponseInfoExtensions
    {
        public static bool IsSuccess(this IProductReviewResponseInfo producReviewResponseInfo)
        {
            return producReviewResponseInfo != null && producReviewResponseInfo.Type == ProductReviewResponseType.Success;
        }
    }
}