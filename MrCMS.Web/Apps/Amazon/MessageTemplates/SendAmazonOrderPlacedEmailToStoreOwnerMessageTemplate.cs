using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.MessageTemplates
{
    [FriendlyClassName("Send Amazon Order Placed Email To Store Owner Message Template")]
    public class SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate : MessageTemplate, IMessageTemplate<AmazonOrder>
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
            return messageTemplateParser.GetAllTokens<AmazonOrder>();
        }
    }
}