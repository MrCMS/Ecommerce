using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class ShippingCountryIs : DiscountLimitation
    {
        public override string DisplayName
        {
            get { return "Shipping country is " + ShippingCountryCode; }
        }

        [DisplayName("Country")]
        public virtual string ShippingCountryCode { get; set; }
    }
}