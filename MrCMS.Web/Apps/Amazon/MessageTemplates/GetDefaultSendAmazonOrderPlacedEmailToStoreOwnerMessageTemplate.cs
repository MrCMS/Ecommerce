using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Amazon.MessageTemplates
{
    public class GetDefaultSendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate :
        GetDefaultTemplate<SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate>
    {
        private Site _site;

        public GetDefaultSendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate(Site site)
        {
            _site = site;
        }

        public override SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate Get()
        {
            var fromName = _site.Name;

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
    }
}