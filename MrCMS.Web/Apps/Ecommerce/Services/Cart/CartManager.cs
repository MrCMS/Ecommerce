using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartManager : ICartManager
    {
        public const string CurrentShippingAddressKey = "current.shipping-address";
        public const string CurrentBillingAddressSameAsShippingAddressKey =
            "current.billing-address-same-as-shipping-address";
        public const string CurrentBillingAddressKey = "current.billing-address";
        public const string CurrentShippingMethodIdKey = "current.shipping-method-id";
        public const string CurrentOrderEmailKey = "current.order-email";
        public const string CurrentDiscountCodeKey = "current.discount-code";
        public const string CurrentPaymentMethodKey = "current.payment-method";
        public const string CurrentCountryIdKey = "current.country-id";

        private readonly CartModel _cart;
        private readonly ISession _session;

        public CartManager(CartModel cart, ISession session)
        {
            _cart = cart;
            _session = session;
        }

        public void AddToCart(ProductVariant item, int quantity)
        {

            var existingItem = _cart.Items.FirstOrDefault(cartItem => cartItem.Item.SKU == item.SKU);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
            {
                existingItem = new CartItem { Item = item, Quantity = quantity, UserGuid = CurrentRequestData.UserGuid };
                _cart.Items.Add(existingItem);
            }
            _session.Transact(session => session.SaveOrUpdate(existingItem));
        }

        public void Delete(CartItem item)
        {
            _cart.Items.Remove(item);
            _session.Transact(session => session.Delete(item));
        }

        public void UpdateQuantity(CartItem item, int quantity)
        {
            item.Quantity = quantity;

            _session.Transact(session => session.Update(item));
        }

        public void UpdateQuantities(List<CartUpdateValue> quantities)
        {
            _session.Transact(session =>
            {
                foreach (var value in quantities)
                {
                    var cartItem = _cart.Items.FirstOrDefault(item => item.Id == value.ItemId);

                    if (cartItem != null)
                    {
                        if (value.Quantity <= 0)
                            session.Delete(cartItem);
                        else
                        {
                            cartItem.Quantity = value.Quantity;
                            session.Update(cartItem);
                        }
                    }
                }
            });
        }

        public void EmptyBasket()
        {
            _cart.Items.ForEach(Delete);
        }

        public void SetShippingAddress(Address address)
        {
            SetSessionItem(CurrentShippingAddressKey, address);
        }

        public void SetBillingAddress(Address address)
        {
            SetSessionItem(CurrentBillingAddressKey, address);
        }

        public void SetBillingAddressSameAsShippingAddress(bool value)
        {
            SetSessionItem(CurrentBillingAddressSameAsShippingAddressKey, value);
        }

        public void SetDiscountCode(string code)
        {
            SetSessionItem(CurrentDiscountCodeKey, code);
        }

        public void SetOrderEmail(string email)
        {
            SetSessionItem(CurrentOrderEmailKey, email);
        }

        public void SetPaymentMethod(string methodName)
        {
            SetSessionItem(CurrentPaymentMethodKey, methodName);
        }

        public void SetShippingInfo(ShippingCalculation shippingCalculation)
        {
            if (shippingCalculation == null) return;

            SetSessionItem(CurrentShippingMethodIdKey, shippingCalculation.ShippingMethod.Id);
            SetSessionItem(CurrentCountryIdKey, shippingCalculation.Country.Id);
        }

        private static void SetSessionItem<T>(string key, T item)
        {
            if (CurrentRequestData.CurrentContext.Session != null)
                CurrentRequestData.CurrentContext.Session[key] = item;
        }
    }
}