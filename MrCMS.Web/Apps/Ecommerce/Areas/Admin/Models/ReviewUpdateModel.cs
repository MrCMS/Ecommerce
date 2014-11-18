using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class ReviewUpdateModel
    {
        public int ReviewId { get; set; }

        public string CurrentOperation { get; set; }

        public bool Approved { get; set; }
    }
}