using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    [FriendlyClassName("Order Shipped To Customer Message Template")]
    public class OrderShippedToCustomerMessageTemplate : MessageTemplate, IMessageTemplate<User>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;
            return new OrderShippedToCustomerMessageTemplate
            {
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Shipped", fromName),
                Body = "Your order was successfully shipped.",
                IsHtml = false
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<Order>();
        }
    }
}