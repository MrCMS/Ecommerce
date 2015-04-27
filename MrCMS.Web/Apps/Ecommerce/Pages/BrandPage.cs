using System.Collections.Generic;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class BrandPage : TextPage
    {
        public BrandPage()
        {
            Products = new List<Product>();
        }

        public virtual IList<Product> Products { get; set; }
    }
}