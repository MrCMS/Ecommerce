using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website;
using NHibernate;
using System;
using NHibernate.Criterion;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetCartImpl : IGetCart
    {
        private readonly ISession _session;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IOrderShippingService _orderShippingService;

        public GetCartImpl(ISession session, IPaymentMethodService paymentMethodService, IOrderShippingService orderShippingService)
        {
            _session = session;
            _paymentMethodService = paymentMethodService;
            _orderShippingService = orderShippingService;
        }

        public CartModel GetCart()
        {
            //var address = GetShippingAddress() ?? new Address();
            //if (GetCountry() != null)
            //    address.Country = GetCountry();
            var availablePaymentMethods = _paymentMethodService.GetAllAvailableMethods();
            var cart = new CartModel
                           {
                               User = CurrentRequestData.CurrentUser,
                               UserGuid = CurrentRequestData.UserGuid,
                               Items = GetItems(),
                               ShippingAddress = GetShippingAddress(),
                               BillingAddress = GetBillingAddress(),
                               Country = GetCountry(),
                               OrderEmail = GetOrderEmail(),
                               DiscountCode = GetDiscountCode(),
                               Discount = GetDiscount(),
                               AnyStandardPaymentMethodsAvailable = _paymentMethodService.AnyStandardMethodsEnabled(),
                               PayPalExpressAvailable = _paymentMethodService.PayPalExpressCheckoutIsEnabled(),
                               AvailablePaymentMethods = availablePaymentMethods,
                               PaymentMethod = GetPaymentMethod() ?? (availablePaymentMethods.Count() == 1 ? availablePaymentMethods.First().Name : null)
                           };
            cart.ShippingMethod = GetShippingMethod(cart);
            return cart;
        }

        private List<CartItem> GetItems()
        {
            return
                _session.QueryOver<CartItem>()
                        .Where(item => item.UserGuid == CurrentRequestData.UserGuid)
                        .Cacheable()
                        .List().ToList();
        }

        private Discount GetDiscount()
        {
            if (!String.IsNullOrWhiteSpace(GetDiscountCode()))
                return
                    _session.QueryOver<Discount>()
                            .Where(item => item.Code.IsInsensitiveLike(GetDiscountCode(), MatchMode.Exact))
                            .Cacheable()
                            .SingleOrDefault();
            else
                return null;
        }

        private Address GetShippingAddress()
        {
            return GetSessionValue<Address>(CartManager.CurrentShippingAddressKey);
        }

        private Address GetBillingAddress()
        {
            return GetSessionValue<Address>(CartManager.CurrentBillingAddressKey);
        }

        private ShippingMethod GetShippingMethod(CartModel cart)
        {
            var id = GetSessionValue<int>(CartManager.CurrentShippingMethodIdKey);

            return _session.Get<ShippingMethod>(id) ??
                   _orderShippingService.GetDefaultShippingMethod(cart);
        }

        private string GetOrderEmail()
        {
            return GetSessionValue<string>(CartManager.CurrentOrderEmailKey);
        }

        private string GetDiscountCode()
        {
            return GetSessionValue<string>(CartManager.CurrentDiscountCodeKey);
        }

        public string GetPaymentMethod()
        {
            return GetSessionValue<string>(CartManager.CurrentPaymentMethodKey);
        }

        private Country GetCountry()
        {
            var id = GetSessionValue<int>(CartManager.CurrentCountryIdKey);

            return _session.Get<Country>(id) ??
                   _session.QueryOver<Country>().Cacheable().Take(1).SingleOrDefault();
        }

        private T GetSessionValue<T>(string key)
        {
            if (CurrentRequestData.CurrentContext.Session != null)
            {
                try { return (T)Convert.ChangeType(CurrentRequestData.CurrentContext.Session[key], typeof(T)); }
                catch { }
            }
            return default(T);
        }
    }
}