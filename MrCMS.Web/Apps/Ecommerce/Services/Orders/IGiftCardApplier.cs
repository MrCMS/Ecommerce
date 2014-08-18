using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IGiftCardApplier
    {
        Order Apply(List<GiftCard> appliedGiftCards, Order order);
    }

    public class GiftCardApplier : IGiftCardApplier
    {
        public Order Apply(List<GiftCard> appliedGiftCards, Order order)
        {
            if (!appliedGiftCards.Any())
            {
                return order;
            }
            var totalToApply = order.Total;
            foreach (var card in appliedGiftCards)
            {
                bool breakAfterThis = false;
                var availableAmount = card.AvailableAmount;
                decimal amount;
                if (availableAmount > totalToApply)
                {
                    breakAfterThis = true;
                    amount = totalToApply;
                }
                else
                {
                    amount = availableAmount;
                }
                totalToApply -= amount;
                var giftCardUsage = new GiftCardUsage
                {
                    GiftCard = card,
                    Order = order,
                    Amount = amount
                };
                order.GiftCardUsages.Add(giftCardUsage);
                card.GiftCardUsages.Add(giftCardUsage);
                if (breakAfterThis)
                    break;
            }

            return order;
        }
    }
}