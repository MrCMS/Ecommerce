using System.Web.Mvc;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
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