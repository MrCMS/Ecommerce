using System.Collections.Generic;
using System.ComponentModel;
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

        [DisplayName("Abstract")]
        public virtual string BrandAbstract { get; set; }
    }
}