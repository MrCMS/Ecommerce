using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class Brand : TextPage
    {
        public Brand()
        {
            Products = new List<Product>();
        }

        public virtual IList<Product> Products { get; set; }

        [DisplayName("Abstract")]
        public virtual string BrandAbstract { get; set; }
    }
}