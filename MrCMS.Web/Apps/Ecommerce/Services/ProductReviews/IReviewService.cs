using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.ProductReviews
{
    public interface IReviewService
    {
        Review GetById(int id);

        IList<Review> GetAll();

        void Add(Review review);

        void Update(Review review);

        void Delete(Review review);

        List<SelectListItem> GetRatingOptions();

        IPagedList<Review> GetReviewsByProductVariantId(ProductVariant productVariant, int page, int pageSize = 10);

        decimal GetAverageRatingsByProductVariant(ProductVariant productVariant);
        IPagedList<Review> GetReviewsByUser(User user, int page, int pageSize = 10);

        IPagedList<Review> GetPaged(int pageNum, string search, int pageSize = 10);

    }
}