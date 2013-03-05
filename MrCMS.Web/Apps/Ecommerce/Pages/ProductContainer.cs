using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class ProductContainer : Webpage, IUniquePage, IDocumentContainer<Product>
    {
        public virtual int PageSize { get; set; }
        public virtual bool AllowPaging { get; set; }
        public virtual IEnumerable<Product> ChildItems { get { return Children.OfType<Product>(); } }
    }
}