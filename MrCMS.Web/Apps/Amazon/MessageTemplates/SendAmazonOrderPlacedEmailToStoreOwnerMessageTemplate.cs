using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.MessageTemplates
{
    [FriendlyClassName("Send Amazon Order Placed Email To Store Owner Message Template")]
    public class SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate : MessageTemplate, IMessageTemplate<Order>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;

            return new SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate
            {
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{UserName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Amazon Order Placed", fromName),
                Body = "Amazon Order (ID:{Id}) was successfully placed.",
                IsHtml = false
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<Order>();
        }
    }

    public class AmazonOrderTokenProvider : ITokenProvider<Order>
    {
        private readonly IAmazonOrderService _amazonOrderService;

        public AmazonOrderTokenProvider(IAmazonOrderService amazonOrderService)
        {
            _amazonOrderService = amazonOrderService;
        }

        public IDictionary<string, Func<Order, string>> Tokens
        {
            get
            {
                return new Dictionary<string, Func<Order, string>>
                    {
                        {
                            "AmazonOrderId", order =>
                                {
                                    var byOrderId = _amazonOrderService.GetByOrderId(order.Id);
                                    return byOrderId != null ? byOrderId.AmazonOrderId : string.Empty;
                                }

                        },
                        {
                            "FulfillmentChannel", order =>
                                {
                                    var byOrderId = _amazonOrderService.GetByOrderId(order.Id);
                                    if (byOrderId != null)
                                    {
                                        var fullFilmentChannel = byOrderId.FulfillmentChannel.HasValue
                                                                     ? byOrderId.FulfillmentChannel.Value.ToString()
                                                                     : string.Empty;
                                        return !string.IsNullOrEmpty(fullFilmentChannel)
                                                   ? fullFilmentChannel
                                                   : string.Empty;
                                    }
                                    return string.Empty;
                                }}
                    };
            }
        }
    }
}