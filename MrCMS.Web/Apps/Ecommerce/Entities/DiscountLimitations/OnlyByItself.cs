using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class OnlyByItself : DiscountLimitation
    {
        public override string DisplayName
        {
            get { return "Only by itself"; }
        }
    }
}