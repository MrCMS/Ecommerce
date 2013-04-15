using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PaypalExpress
{
    public interface IPaypalExpressCheckoutService
    {
        string GetRedirectUrl(CartModel cart);
    }

    public class PaypalExpressCheckoutService : IPaypalExpressCheckoutService
    {
        private readonly PaymentGatewaySettings _paymentGatewaySettings;

        public PaypalExpressCheckoutService(PaymentGatewaySettings paymentGatewaySettings)
        {
            _paymentGatewaySettings = paymentGatewaySettings;
        }

        public string GetRedirectUrl(CartModel cart)
        {
            var wrapper = new SetExpressCheckoutReq();
            var type = new SetExpressCheckoutRequestType();
            var addressType = new AddressType
                                  {
                                      Street1 = cart.ShippingAddress.Address1, Street2 = cart.ShippingAddress.Address2, CityName = cart.ShippingAddress.City, PostalCode = cart.ShippingAddress.PostalCode, CountryName = cart.ShippingAddress.Country.Name, StateOrProvince = cart.ShippingAddress.ToString(), Phone = cart.ShippingAddress.PhoneNumber, Name = cart.ShippingAddress.Name
                                  };
            var requestDetails = new SetExpressCheckoutRequestDetailsType
                                     {
                                         Address = addressType,
                                         CallbackURL = "/paypal-order-response?token=" + cart.UserGuid,
                                         PaymentDetails = new List<PaymentDetailsType>
                                                              {
                                                                  new PaymentDetailsType
                                                                      {
                                                                          AllowedPaymentMethod =
                                                                              AllowedPaymentMethodType.DEFAULT,
                                                                          ItemTotal =
                                                                              new BasicAmountType(CurrencyCodeType.GBP,
                                                                                                  (cart.Total -
                                                                                                   cart.ShippingTotal
                                                                                                       .GetValueOrDefault
                                                                                                       ()).ToString()),
                                                                          ShippingTotal =
                                                                              new BasicAmountType(CurrencyCodeType.GBP,
                                                                                                  cart.ShippingTotal
                                                                                                      .GetValueOrDefault
                                                                                                      ().ToString()),
                                                                          OrderTotal =
                                                                              new BasicAmountType(CurrencyCodeType.GBP,
                                                                                                  cart.Total.ToString()),
                                                                          PaymentDetailsItem = GetItems(cart),
                                                                          TaxTotal =
                                                                              new BasicAmountType(CurrencyCodeType.GBP,
                                                                                                  cart.Tax.ToString()),
                                                                          ShipToAddress = addressType,

                                                                      }
                                                              }
                                     };
            
            type.SetExpressCheckoutRequestDetails = requestDetails;

            wrapper.SetExpressCheckoutRequest = type;

            var service = new PayPalAPIInterfaceServiceService();
            SetExpressCheckoutResponseType setECResponse = service.SetExpressCheckout(wrapper);

            if (!setECResponse.Ack.Equals(AckCodeType.FAILURE))
            {
                return _paymentGatewaySettings.PaypalExpressCheckoutUrl + setECResponse.Token;
            }
            return null;
        }

        private List<PaymentDetailsItemType> GetItems(CartModel cart)
        {
            return cart.Items.Select(item => new PaymentDetailsItemType
                                                 {
                                                     Amount =
                                                         new BasicAmountType(CurrencyCodeType.GBP, item.Price.ToString()),
                                                     Tax =
                                                         new BasicAmountType(CurrencyCodeType.GBP, item.Tax.ToString()),
                                                     Quantity = item.Quantity,
                                                     ItemWeight = new MeasureType("kg", item.Weight),
                                                     Name = item.Name,
                                                     PromoCode = cart.DiscountCode
                                                 }).ToList();
        }
    }
}