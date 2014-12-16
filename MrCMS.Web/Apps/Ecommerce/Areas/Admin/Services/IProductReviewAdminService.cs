using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IProductReviewAdminService
    {
        void BulkAction(ReviewUpdateModel model);

        List<SelectListItem> GetApprovalOptions();

        IPagedList<ProductReview> Search(ProductReviewSearchQuery query);
    }
}