using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultSendOrderPlacedEmailToCustomerMessageTemplate :
        GetDefaultTemplate<SendOrderPlacedEmailToCustomerMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultSendOrderPlacedEmailToCustomerMessageTemplate(Site site)
        {
            _site = site;
        }

        public override SendOrderPlacedEmailToCustomerMessageTemplate Get()
        {
            string fromName = _site.Name;

            return new SendOrderPlacedEmailToCustomerMessageTemplate
            {
                FromName = fromName,
                ToAddress = "{OrderEmail}",
                ToName = "{CustomerName}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Placed", fromName),
                Body =
                    "<p>Dear {CustomerName},</p> <p>Your order {Id} was successfully placed. </p> Items ordered: <p>{ShoppingCartHtml}</p> <p>Shipping Address: <br />{ShippingAddressFormattedHtml}</p><p>Thank you for your custom.</p>",
                IsHtml = true
            };
        }
    }
}