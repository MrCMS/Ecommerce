using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultSendOrderPartiallyRefundedEmailToCustomerMessageTemplate :
        GetDefaultTemplate<SendOrderPartiallyRefundedEmailToCustomerMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultSendOrderPartiallyRefundedEmailToCustomerMessageTemplate(Site site)
        {
            _site = site;
        }

        public override SendOrderPartiallyRefundedEmailToCustomerMessageTemplate Get()
        {
            string fromName = _site.Name;

            return new SendOrderPartiallyRefundedEmailToCustomerMessageTemplate
            {
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{CustomerName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Partially Refunded", fromName),
                Body = "Your order (ID:{Id}) was partially refunded.",
                IsHtml = false
            };
        }
    }
}