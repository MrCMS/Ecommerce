using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class CartSubtotalGreaterThanX : DiscountLimitation
    {
        public override string DisplayName
        {
            get { return string.Format("Cart product subtotal is greater than {0:00}", CartSubtotalGreaterThanValue); }
        }

        [DisplayName("Value")]
        public virtual decimal CartSubtotalGreaterThanValue { get; set; }
    }
}