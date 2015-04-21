using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultSendOrderPlacedEmailToStoreOwnerMessageTemplate : GetDefaultTemplate<SendOrderPlacedEmailToStoreOwnerMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultSendOrderPlacedEmailToStoreOwnerMessageTemplate(Site site)
        {
            _site = site;
        }

        public override SendOrderPlacedEmailToStoreOwnerMessageTemplate Get()
        {
            var fromName = _site.Name;

            return new SendOrderPlacedEmailToStoreOwnerMessageTemplate
            {
                FromName = fromName,
                ToAddress = "you@example.com",
                ToName = "Site Owner",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Order Placed", fromName),
                Body = "<p>Dear store owner,</p> <p>Order {Id} was successfully placed. </p> Items ordered: <p>{ShoppingCartHtml}</p> <p>Shipping Address: <br />{ShippingAddressFormattedHtml}</p>",
                IsHtml = true
            };
        }
    }
}