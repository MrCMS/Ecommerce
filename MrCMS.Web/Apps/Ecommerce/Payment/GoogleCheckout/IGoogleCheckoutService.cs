using GCheckout.Checkout;
using GCheckout.Util;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.GoogleCheckout
{
    public interface IGoogleCheckoutService
    {
        string GetRedirectUrl(CartModel cart);
    }

    public class GoogleCheckoutService : IGoogleCheckoutService
    {
        private readonly PaymentSettings _paymentSettings;
        private readonly IDocumentService _documentService;

        public GoogleCheckoutService(PaymentSettings paymentSettings, IDocumentService documentService)
        {
            _paymentSettings = paymentSettings;
            _documentService = documentService;
        }

        public string GetRedirectUrl(CartModel cart)
        {
            var checkoutShoppingCartRequest =
                new CheckoutShoppingCartRequest(_paymentSettings.GoogleCheckoutMerchantID,
                                                _paymentSettings.GoogleCheckoutMerchantKey,
                                                _paymentSettings.GoogleCheckoutEnvironment, "GBP", 0, false);

            foreach (var cartItem in cart.Items)
            {
                checkoutShoppingCartRequest.AddItem(cartItem.Name, string.Empty, cartItem.Item.SKU, cartItem.UnitPrice,
                                                    cartItem.Quantity);
            }
            checkoutShoppingCartRequest.AddFlatRateShippingMethod(cart.ShippingMethod.Name,
                                                                  cart.ShippingTotal.GetValueOrDefault());
            var homePage = _documentService.GetDocumentByUrl<Webpage>("/");
            checkoutShoppingCartRequest.ContinueShoppingUrl = homePage.AbsoluteUrl;
            var cartPage = _documentService.GetUniquePage<Cart>();
            checkoutShoppingCartRequest.EditCartUrl = cartPage.AbsoluteUrl;

            var gCheckoutResponse = checkoutShoppingCartRequest.Send();

            return gCheckoutResponse.IsGood
                       ? gCheckoutResponse.RedirectUrl
                       : null;
        }
    }
}