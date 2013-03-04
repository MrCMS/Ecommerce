using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class Category : Webpage
    {
        public Category()
        {
            Products = new List<Product>();
        }

        private string _nestedName;

        public virtual string NestedName
        {
            get { return _nestedName ?? (_nestedName = GetNestedName()); }
        }

        private string GetNestedName()
        {
            var categories = ActivePages.TakeWhile(webpage => webpage.Unproxy() is Category).Reverse();

            return string.Join(" > ", categories.Select(webpage => webpage.Name));
        }

        public virtual IList<Product> Products { get; set; }
    }
}