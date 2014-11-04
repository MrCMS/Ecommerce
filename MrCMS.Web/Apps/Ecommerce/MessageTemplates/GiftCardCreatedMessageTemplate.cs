using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GiftCardCreatedMessageTemplate : MessageTemplate, IMessageTemplate<GiftCard>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;
            return new GiftCardCreatedMessageTemplate
            {
                FromAddress = "test@example.com",
                FromName = fromName,
                ToAddress = "{RecipientEmail}",
                ToName = "{RecipientName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Your Gift Card", fromName),
                Body = "<p>You have purchased a gift card with the code {Code}</p>",
                IsHtml = true
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<GiftCard>();
        }
    }
}