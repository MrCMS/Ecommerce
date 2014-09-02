using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Models
{
    public class PageModel
    {
        public TextPage HomePage { get; set; }
        public ProductSearch ProductSearch { get; set; }
        public ProductAddedToCart ProductAddedToCart { get; set; }
    }

    public class LayoutModel
    {
        public Layout HomeLayout { get; set; }
        public Layout EcommerceLayout { get; set; }
        public Layout ContentLayout { get; set; }
        public Layout ProductLayout { get; set; }
        public Layout SearchLayout { get; set; }
        public Layout CheckoutLayout { get; set; }

    }

}