using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    [DisplayName("Fixed Amount")]
    public class OrderSubTotalFixedAmount : DiscountApplication
    {
        [DisplayName("Discount Amount")]
        public virtual decimal DiscountAmount { get; set; }

        public override string DisplayName
        {
            get { return string.Format("{0:00} from order subtotal", DiscountAmount); }
        }
    }
}