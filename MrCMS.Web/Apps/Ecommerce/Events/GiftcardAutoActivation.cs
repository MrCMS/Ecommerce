using System;
using System.Linq;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class GiftcardAutoActivation : IOnAdded<GiftCard>, IOnUpdated<GiftCard>, IOnUpdated<Order>
    {
        private readonly GiftCardSettings _settings;
        private readonly ISession _session;
        private readonly IMessageParser<GiftCardCreatedMessageTemplate, GiftCard> _messageParser;

        public GiftcardAutoActivation(GiftCardSettings settings, ISession session, IMessageParser<GiftCardCreatedMessageTemplate, GiftCard> messageParser)
        {
            _settings = settings;
            _session = session;
            _messageParser = messageParser;
        }

        public void ActivateCard(GiftCard card)
        {
            card.Status = GiftCardStatus.Activated;
            _session.Transact(session =>
            {
                if (card.GiftCardType == GiftCardType.Virtual)
                {
                    var message = _messageParser.GetMessage(card);
                    _messageParser.QueueMessage(message);
                }
                card.NotificationSent = true;
                session.Update(card);
            });
        }

        public void Execute(OnAddedArgs<GiftCard> args)
        {
            GiftCard giftCard = args.Item;
            Order order;
            if (ShouldCheck(giftCard, out order)) return;
            ActivateIfRequired(giftCard, order);
        }

        public void Execute(OnUpdatedArgs<GiftCard> args)
        {
            GiftCard giftCard = args.Item;
            Order order;
            if (ShouldCheck(giftCard, out order)) return;
            ActivateIfRequired(giftCard, order);
        }

        private void ActivateIfRequired(GiftCard giftCard, Order order)
        {
            switch (_settings.ActivateOn)
            {
                case ActivateOn.Payment:
                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ActivateCard(giftCard);
                    break;
                case ActivateOn.Shipping:
                    if (giftCard.GiftCardType == GiftCardType.Virtual)
                        ActivateCard(giftCard);
                    else if (order.ShippingStatus == ShippingStatus.Shipped)
                    {
                        ActivateCard(giftCard);
                    }
                    break;
                case ActivateOn.OrderCompletion:
                    ActivateCard(giftCard);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool ShouldCheck(GiftCard giftCard, out Order order)
        {
            order = null;
            if (giftCard == null || giftCard.Status != GiftCardStatus.Unactivated)
                return true;
            var orderLine = giftCard.OrderLine;
            if (orderLine == null)
                return true;
            order = orderLine.Order;
            if (order == null)
                return true;
            return false;
        }

        public void Execute(OnUpdatedArgs<Order> args)
        {
            var order = args.Item;
            if (order == null)
                return;
            var giftCards = order.OrderLines.SelectMany(line => line.GiftCards).ToList();
            foreach (var giftCard in giftCards.Where(card => card.Status == GiftCardStatus.Unactivated))
            {
                ActivateIfRequired(giftCard, order);
            }
        }
    }
}