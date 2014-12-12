using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class ItemIsInCategory : DiscountLimitation
    {
        [DisplayName("Category IDs (comma delimited list)")]
        public virtual string CategoryIds { get; set; }

        public override string DisplayName
        {
            get { return "Item has on of the following category ids: " + CategoryIds; }
        }
    }
}