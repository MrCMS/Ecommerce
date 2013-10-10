using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    [FriendlyClassName("Send Order Shipped Email To Customer Message Template")]
    public class SendOrderShippedEmailToCustomerMessageTemplate : MessageTemplate, IMessageTemplate<Order>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;

            return new SendOrderShippedEmailToCustomerMessageTemplate
            {
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{CustomerName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Shipped", fromName),
                Body = "<p>Dear {CustomerName},</p> <p>Your order {Id} was successfully shipped to: </p><p>{ShippingAddress}</p><p>Thank you for your custom.</p>",
                IsHtml = true
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<Order>();
        }
    }
}