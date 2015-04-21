using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultGiftCardCreatedMessageTemplate : GetDefaultTemplate<GiftCardCreatedMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultGiftCardCreatedMessageTemplate(Site site)
        {
            _site = site;
        }

        public override GiftCardCreatedMessageTemplate Get()
        {
            var fromName = _site.Name;
            return new GiftCardCreatedMessageTemplate
            {
                FromAddress = "test@example.com",
                FromName = fromName,
                ToAddress = "{RecipientEmail}",
                ToName = "{RecipientName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("Someone has given you a Gift Card for {0}", fromName),
                Body = "<p>{SenderName} has send you a gift card for {SiteName}. To use this gift card, please enter code: {Code} at the checkout.</p>",
                IsHtml = true
            };
        }
    }
}