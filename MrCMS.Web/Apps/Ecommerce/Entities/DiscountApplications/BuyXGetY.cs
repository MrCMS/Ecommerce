using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class BuyXGetYFree : CartItemBasedDiscountApplication
    {
        [DisplayName("Buy Amount")]
        public virtual int BuyAmount { get; set; }

        [DisplayName("Free Amount")]
        public virtual int FreeAmount { get; set; }

        public override string DisplayName
        {
            get { return string.Format("Buy {0} get {1} free", BuyAmount, FreeAmount); }
        }
    }
}