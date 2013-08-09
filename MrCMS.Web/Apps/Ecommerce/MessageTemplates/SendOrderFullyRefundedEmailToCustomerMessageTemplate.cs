using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    [FriendlyClassName("Send Order Fully Refunded Email To Customer Message Template")]
    public class SendOrderFullyRefundedEmailToCustomerMessageTemplate : MessageTemplate, IMessageTemplate<Order>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;
            var fromAddress = MrCMSApplication.Get<MailSettings>().SystemEmailAddress;

            return new SendOrderFullyRefundedEmailToCustomerMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{UserName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Fully Refunded", fromName),
                Body = "Your order (ID:{Id}) was fully refunded.",
                IsHtml = false
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<Order>();
        }
    }
}