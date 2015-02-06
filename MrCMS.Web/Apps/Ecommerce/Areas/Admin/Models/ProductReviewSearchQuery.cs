using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class ProductReviewSearchQuery
    {
        public ProductReviewSearchQuery()
        {
            Page = 1;
        }

        public int Page { get; set; }
        [DisplayName("Approval Status")]
        public ApprovalStatus ApprovalStatus { get; set; }

        public string Email { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        public string Title { get; set; }

        [DisplayName("Date From")]
        public DateTime? DateFrom { get; set; }

        [DisplayName("Date To")]
        public DateTime? DateTo { get; set; }
    }
}