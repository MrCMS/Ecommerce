using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public interface IProductReviewUIService
    {
        ProductReview GetById(int id);

        IList<ProductReview> GetAll();

        void Add(ProductReview productReview);

        void Update(ProductReview productReview);

        void Delete(ProductReview productReview);

        List<SelectListItem> GetRatingOptions();

        IPagedList<ProductReview> GetReviewsByProductVariantId(ProductVariant productVariant, int page, int pageSize = 10);

        decimal GetAverageRatingsByProductVariant(ProductVariant productVariant);
        IPagedList<ProductReview> GetReviewsByUser(User user, int page, int pageSize = 10);

        IPagedList<ProductReview> GetPaged(int pageNum, string search, int pageSize = 10);

    }
}