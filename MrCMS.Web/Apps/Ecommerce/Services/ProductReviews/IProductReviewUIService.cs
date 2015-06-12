using MrCMS.Entities.People;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public interface IProductReviewUIService
    {
        void Add(ProductReview productReview);

        IPagedList<ProductReview> GetReviewsForVariant(ProductVariant productVariant, int page = 1, int pageSize = 10);
        decimal GetAverageRatingForProductVariant(ProductVariant productVariant);
        IPagedList<ProductReview> GetReviewsByUser(User user, int page = 1, int pageSize = 10);
    }
}