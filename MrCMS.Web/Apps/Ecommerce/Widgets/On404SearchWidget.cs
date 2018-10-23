using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    [WidgetOutputCacheable]
    public class On404SearchWidget : Widget
    {
        public On404SearchWidget()
        {
            MaxProductsToShow = 12;
        }

        [AllowHtml]
        public virtual string Text { get; set; }

        public virtual int MaxProductsToShow { get; set; }
    }
}