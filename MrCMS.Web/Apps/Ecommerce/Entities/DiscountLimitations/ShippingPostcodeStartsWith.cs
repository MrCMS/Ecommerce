using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class ShippingPostcodeStartsWith : DiscountLimitation
    {
        public override string DisplayName
        {
            get { return "Shipping postcode starts with " + ShippingPostcode; }
        }

        [DisplayName("Country")]
        public virtual string ShippingPostcode { get; set; }
    }
}