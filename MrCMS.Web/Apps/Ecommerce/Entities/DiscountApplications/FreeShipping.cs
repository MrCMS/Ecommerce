using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class FreeShipping : DiscountApplication
    {
        public override string DisplayName
        {
            get { return "Free shipping"; }
        }
    }
}