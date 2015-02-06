using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProductSearchQuery : ICloneable
    {
        public ProductSearchQuery()
        {
            Options = new List<string>();
            Specifications = new List<int>();
            Page = 1;
            PageSize = 10;
        }

        public List<string> Options { get; set; }
        public List<int> Specifications { get; set; }
        public double PriceFrom { get; set; }
        public double? PriceTo { get; set; }
        public int? CategoryId { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public ProductSearchSort? SortBy { get; set; }
        public int? BrandId { get; set; }

        public object Clone()
        {
            return new ProductSearchQuery
            {
                CategoryId = CategoryId,
                Options = Options,
                Page = Page,
                PageSize = PageSize,
                PriceFrom = PriceFrom,
                PriceTo = PriceTo,
                SearchTerm = SearchTerm,
                SortBy = SortBy,
                Specifications = Specifications,
                BrandId = BrandId
            };
        }
    }
}