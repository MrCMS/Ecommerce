using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class ProductContainer : Webpage, IUniquePage
    {
        public virtual int PageSize { get; set; }
        public virtual bool AllowPaging { get; set; }
    }
}