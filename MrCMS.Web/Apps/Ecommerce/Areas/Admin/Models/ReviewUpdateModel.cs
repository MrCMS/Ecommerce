using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class ReviewUpdateModel
    {
        public List<Review> Reviews { get; set; }

        public ProductReviewOperation CurrentOperation { get; set; }
    }

    public enum ProductReviewOperation
    {
        Approve,
        Reject,
        Delete
    }
}