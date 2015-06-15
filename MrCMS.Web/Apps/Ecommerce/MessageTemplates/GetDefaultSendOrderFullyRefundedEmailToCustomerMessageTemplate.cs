using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultSendOrderFullyRefundedEmailToCustomerMessageTemplate : GetDefaultTemplate<SendOrderFullyRefundedEmailToCustomerMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultSendOrderFullyRefundedEmailToCustomerMessageTemplate(Site site)
        {
            _site = site;
        }

        public override SendOrderFullyRefundedEmailToCustomerMessageTemplate Get()
        {
            var fromName = _site.Name;

            return new SendOrderFullyRefundedEmailToCustomerMessageTemplate
            {
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{CustomerName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Fully Refunded", fromName),
                Body = "Your order (ID:{Id}) was fully refunded.",
                IsHtml = false
            };
        }
    }
}