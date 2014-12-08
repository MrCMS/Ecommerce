using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class OrderTotalFixedAmount : DiscountApplication
    {
        [DisplayName("Discount Amount")]
        public virtual decimal DiscountAmount { get; set; }

        public override string DisplayName
        {
            get { return string.Format("{0:0.00} from order total", DiscountAmount); }
        }
    }
}