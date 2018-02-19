using System.ComponentModel;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    [WidgetOutputCacheable(PerPage = true)]
    public class FeaturedProducts : Widget
    {
        [DisplayName("Featured Products")]
        public virtual string ListOfFeaturedProducts { get; set; }
    }
}