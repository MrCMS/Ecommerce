using MrCMS.Entities.People;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public interface IGetUserProductReviews
    {
        IPagedList<ProductReview> Get(User user, int page = 1);
    }
}