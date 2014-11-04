using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class GiftCardExtensions
    {
        public static bool IsValidToUse(this GiftCard giftCard)
        {
            return giftCard != null && giftCard.Status == GiftCardStatus.Activated && giftCard.AvailableAmount > 0m;
        }
    }
}