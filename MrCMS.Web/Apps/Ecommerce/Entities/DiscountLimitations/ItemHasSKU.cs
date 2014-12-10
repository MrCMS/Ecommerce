using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class ItemHasSKU : DiscountLimitation
    {
        [DisplayName("SKUs (comma delimited list)")]
        public virtual string SKUs { get; set; }

        public override string DisplayName
        {
            get { return "Item has one of the following SKUs: " + SKUs; }
        }
    }
}