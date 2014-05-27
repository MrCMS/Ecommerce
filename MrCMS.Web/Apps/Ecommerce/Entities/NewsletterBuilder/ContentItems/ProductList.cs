using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems
{
    public class ProductList : ContentItem
    {
        //public virtual IList<Product> Products { get; set; }

        public virtual string Products { get; set; }
    }
}