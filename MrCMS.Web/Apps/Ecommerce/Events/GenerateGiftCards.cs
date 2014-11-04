using System;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class GenerateGiftCards : IOnAdded<OrderLine>
    {
        private readonly ISession _session;
        private readonly IGenerateGiftCardCode _generateGiftCardCode;

        public GenerateGiftCards(ISession session, IGenerateGiftCardCode generateGiftCardCode)
        {
            _session = session;
            _generateGiftCardCode = generateGiftCardCode;
        }

        public void Execute(OrderLine orderLine)
        {
            if (orderLine == null || orderLine.ProductVariant == null || string.IsNullOrWhiteSpace(orderLine.Data))
                return;
            var productVariant = orderLine.ProductVariant;
            if (productVariant.IsGiftCard)
            {
                // ensure that duplicate cards aren't generated
                if (orderLine.GiftCards.Any())
                    return;
                try
                {
                    var giftCardInfo = JsonConvert.DeserializeObject<GiftCardInfo>(orderLine.Data);

                    var giftCard = new GiftCard
                    {
                        Code = _generateGiftCardCode.Generate(),
                        GiftCardType = productVariant.GiftCardType,
                        Message = giftCardInfo.Message,
                        RecipientEmail = giftCardInfo.RecipientEmail,
                        RecipientName = giftCardInfo.RecipientName,
                        SenderEmail = giftCardInfo.SenderEmail,
                        SenderName = giftCardInfo.SenderName,
                        Value = productVariant.Price,
                        OrderLine = orderLine,
                    };
                    orderLine.GiftCards.Add(giftCard);
                    _session.Transact(session => session.Save(giftCard));
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                }
            }
        }

        public void Execute(OnAddedArgs<OrderLine> args)
        {
            _session.Transact(session => Execute(args.Item));
        }
    }
}