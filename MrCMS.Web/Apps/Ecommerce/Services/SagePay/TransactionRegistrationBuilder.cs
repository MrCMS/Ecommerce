using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class TransactionRegistrationBuilder : ITransactionRegistrationBuilder
    {
        private readonly SagePaySettings _sagePaySettings;
        private readonly ISagePayItemCreator _sagePayItemCreator;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly ISagePayUrlResolver _sagePayUrlResolver;
        private readonly Site _site;

        public TransactionRegistrationBuilder(SagePaySettings sagePaySettings,
                                              ISagePayItemCreator sagePayItemCreator,
                                              EcommerceSettings ecommerceSettings,
                                              ISagePayUrlResolver sagePayUrlResolver,
            Site site)
        {
            _sagePaySettings = sagePaySettings;
            _sagePayItemCreator = sagePayItemCreator;
            _ecommerceSettings = ecommerceSettings;
            _sagePayUrlResolver = sagePayUrlResolver;
            _site = site;
        }

        public TransactionRegistration BuildRegistration(CartModel cartModel)
        {
            var shoppingBasket = _sagePayItemCreator.GetShoppingBasket(cartModel);
            var billingAddress = _sagePayItemCreator.GetAddress(cartModel.BillingAddress);
            var deliveryAddress = _sagePayItemCreator.GetAddress(cartModel.ShippingAddress);
            var transactionRegistration = new TransactionRegistration
                                              {
                                                  AllowGiftAid = _sagePaySettings.AllowGiftAidString,
                                                  Amount = shoppingBasket.Total,
                                                  Apply3DSecure = _sagePaySettings.Apply3DSecure,
                                                  ApplyAVSCV2 = _sagePaySettings.ApplyAVSCV2,
                                                  Basket = shoppingBasket.ToString(),
                                                  BillingAddress1 = billingAddress.Address1,
                                                  BillingAddress2 = billingAddress.Address2,
                                                  BillingCity = billingAddress.City,
                                                  BillingCountry = billingAddress.Country,
                                                  BillingFirstNames = billingAddress.Firstnames,
                                                  BillingPhone = billingAddress.Phone,
                                                  BillingPostcode = billingAddress.PostCode,
                                                  BillingState = billingAddress.State,
                                                  BillingSurname = billingAddress.Surname,
                                                  Currency = _ecommerceSettings.CurrencyCode,
                                                  CustomerEMail = cartModel.OrderEmail,
                                                  DeliveryAddress1 = deliveryAddress.Address1,
                                                  DeliveryAddress2 = deliveryAddress.Address2,
                                                  DeliveryCity = deliveryAddress.City,
                                                  DeliveryCountry = deliveryAddress.Country,
                                                  DeliveryFirstNames = deliveryAddress.Firstnames,
                                                  DeliveryPhone = deliveryAddress.Phone,
                                                  DeliveryPostcode = deliveryAddress.PostCode,
                                                  DeliveryState = deliveryAddress.State,
                                                  DeliverySurname = deliveryAddress.Surname,
                                                  Description =
                                                      string.Format("Order from {0}", _site.Name),
                                                  NotificationUrl = _sagePayUrlResolver.BuildNotificationUrl(),
                                                  Profile = _sagePaySettings.PaymentFormProfileString,
                                                  TxType = "PAYMENT",
                                                  VPSProtocol = _sagePaySettings.Protocol,
                                                  Vendor = _sagePaySettings.VendorName,
                                                  VendorTxCode = cartModel.CartGuid.ToString()
                                              };
            return transactionRegistration;
        }
    }
}