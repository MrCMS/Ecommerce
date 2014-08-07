using MrCMS.Entities.Widget;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class FeaturedCategories : Widget
    {
        [DisplayName("Featured Categories")]
        public virtual string ListOfFeaturedCategories { get; set; }
    }
}
