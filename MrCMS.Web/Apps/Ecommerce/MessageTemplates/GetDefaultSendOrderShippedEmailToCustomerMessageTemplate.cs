using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultSendOrderShippedEmailToCustomerMessageTemplate :
        GetDefaultTemplate<SendOrderShippedEmailToCustomerMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultSendOrderShippedEmailToCustomerMessageTemplate(Site site)
        {
            _site = site;
        }

        public override SendOrderShippedEmailToCustomerMessageTemplate Get()
        {
            string fromName = _site.Name;

            return new SendOrderShippedEmailToCustomerMessageTemplate
            {
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{CustomerName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Shipped", fromName),
                Body =
                    "<p>Dear {CustomerName},</p> <p>Your order {Id} was successfully shipped to: </p><p>{ShippingAddress}</p><p>Thank you for your custom.</p>",
                IsHtml = true
            };
        }
    }
}