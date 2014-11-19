using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class UserAccountReviewsModel : AsyncListModel<Review>
    {
        public UserAccountReviewsModel(PagedList<Review> items, int id)
            : base(items, id)
        {
        }
    }
}