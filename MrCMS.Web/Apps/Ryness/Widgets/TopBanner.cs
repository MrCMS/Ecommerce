using System.Web.Mvc;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Ryness.Widgets
{
    public class TopBanner : Widget
    {
        [AllowHtml]
        public virtual string Text { get; set; }
    }
}