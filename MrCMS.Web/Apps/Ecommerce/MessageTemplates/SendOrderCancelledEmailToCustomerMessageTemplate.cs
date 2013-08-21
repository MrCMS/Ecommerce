using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    [FriendlyClassName("Send Order Cancelled Email To Customer Message Template")]
    public class SendOrderCancelledEmailToCustomerMessageTemplate : MessageTemplate, IMessageTemplate<Order>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;
            //var fromAddress = MrCMSApplication.Get<MailSettings>().SystemEmailAddress;

            return new SendOrderCancelledEmailToCustomerMessageTemplate
            {
                //FromAddress = fromAddress,
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{UserName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Cancelled", fromName),
                Body = "Your order (ID:{Id}) was cancelled.",
                IsHtml = false
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<Order>();
        }
    }
}