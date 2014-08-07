using System.ComponentModel;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class FeaturedProducts : Widget
    {
        [DisplayName("Featured Products")]
        public virtual string ListOfFeaturedProducts { get; set; }
    }
}