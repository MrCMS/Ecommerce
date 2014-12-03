using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class CartTotalGreaterThanX : DiscountLimitation
    {
        public override string DisplayName
        {
            get { return string.Format("Cart Total is greater than {0:00}", CartTotalGreaterThanValue); }
        }

        [DisplayName("Value")]
        public virtual decimal CartTotalGreaterThanValue { get; set; }
    }
}