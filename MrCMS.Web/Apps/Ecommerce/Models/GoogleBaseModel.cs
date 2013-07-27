using System;
using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class GoogleBaseModel
    {
        public GoogleBaseModel()
        {
            Items = new PagedList<ProductVariant>(new List<ProductVariant>(),1,10);
            Page = 1;
            Status = string.Empty;
            Category = 0;
            Name = String.Empty;
        }

        public IPagedList<ProductVariant> Items { get; set; }
        public string Name { get; set; }
        public int? Category { get; set; }
        public int Page { get; set; }
        public string Status { get; set; }
    }

}