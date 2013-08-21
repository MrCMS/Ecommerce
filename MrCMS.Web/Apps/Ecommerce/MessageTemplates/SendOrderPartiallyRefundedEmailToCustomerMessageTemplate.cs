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
    [FriendlyClassName("Send Order Partially Refunded Email To Customer Message Template")]
    public class SendOrderPartiallyRefundedEmailToCustomerMessageTemplate : MessageTemplate, IMessageTemplate<Order>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;
            //var fromAddress = MrCMSApplication.Get<MailSettings>().SystemEmailAddress;

            return new SendOrderPartiallyRefundedEmailToCustomerMessageTemplate
            {
                //FromAddress = fromAddress,
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{UserName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Partially Refunded", fromName),
                Body = "Your order (ID:{Id}) was partially refunded.",
                IsHtml = false
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<Order>();
        }
    }
}