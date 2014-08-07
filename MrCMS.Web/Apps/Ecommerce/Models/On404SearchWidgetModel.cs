using System.Linq;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class On404SearchWidgetModel
    {
        public string Text { get; set; }

        public bool AnyProducts
        {
            get { return Products != null && Products.Any(); }
        }

        public IPagedList<Product> Products { get; set; }
    }
}