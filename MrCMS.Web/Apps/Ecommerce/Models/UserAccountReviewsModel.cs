using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class UserAccountReviewsModel : AsyncListModel<ProductReview>
    {
        public UserAccountReviewsModel(PagedList<ProductReview> items, int id)
            : base(items, id)
        {
        }
    }
}