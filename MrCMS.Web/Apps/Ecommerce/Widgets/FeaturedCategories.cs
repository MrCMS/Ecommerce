using MrCMS.Entities.Widget;
using System.ComponentModel;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    [OutputCacheable(PerPage = true)]
    public class FeaturedCategories : Widget
    {
        [DisplayName("Featured Categories")]
        public virtual string ListOfFeaturedCategories { get; set; }
    }
}
