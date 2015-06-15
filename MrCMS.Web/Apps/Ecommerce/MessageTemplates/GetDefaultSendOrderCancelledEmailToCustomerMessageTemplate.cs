using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultSendOrderCancelledEmailToCustomerMessageTemplate :
        GetDefaultTemplate<SendOrderCancelledEmailToCustomerMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultSendOrderCancelledEmailToCustomerMessageTemplate(Site site)
        {
            _site = site;
        }

        public override SendOrderCancelledEmailToCustomerMessageTemplate Get()
        {
            var fromName = _site.Name;

            return new SendOrderCancelledEmailToCustomerMessageTemplate
            {
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{CustomerName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Cancelled", fromName),
                Body = "Your order (ID:{Id}) was cancelled.",
                IsHtml = false
            };
        }
    }
}