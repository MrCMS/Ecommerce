using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class CategoryContainer : Webpage, IUniquePage, IDocumentContainer<Category>
    {
        public virtual int PageSize { get; set; }
        public virtual bool AllowPaging { get; set; }
        public virtual IEnumerable<Category> ChildItems { get { return Children.OfType<Category>(); } }
    }
}