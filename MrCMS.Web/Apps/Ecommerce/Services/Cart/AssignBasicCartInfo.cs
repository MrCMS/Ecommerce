using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignBasicCartInfo : IAssignBasicCartInfo
    {
        private readonly ICartGuidResetter _cartGuidResetter;
        private readonly IGetBillingAddressSameAsShippingAddress _billingAddressSameAsShippingAddress;
        private readonly ICartItemAvailablityService _cartItemAvailabilityService;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IProductPricingMethod _productPricingMethod;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly ISession _session;

        public AssignBasicCartInfo(ISession session, ICartSessionManager cartSessionManager,
            ICartGuidResetter cartGuidResetter,
            IGetBillingAddressSameAsShippingAddress billingAddressSameAsShippingAddress,
            ICartItemAvailablityService cartItemAvailabilityService, EcommerceSettings ecommerceSettings,
            IProductPricingMethod productPricingMethod)
        {
            _session = session;
            _cartSessionManager = cartSessionManager;
            _cartGuidResetter = cartGuidResetter;
            _billingAddressSameAsShippingAddress = billingAddressSameAsShippingAddress;
            _cartItemAvailabilityService = cartItemAvailabilityService;
            _ecommerceSettings = ecommerceSettings;
            _productPricingMethod = productPricingMethod;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            List<CartItemData> cartItems = GetItems(userGuid);
            AssignPricingMethod(cartItems);
            DeleteNullProducts(cartItems);
            AssignAvailability(cartItems);
            cart.CartGuid = GetCartGuid(userGuid);
            cart.User = CurrentRequestData.CurrentUser;
            cart.UserGuid = userGuid;
            cart.Items = cartItems;
            cart.OrderEmail = GetOrderEmail(userGuid);
            cart.GiftMessage = GetGiftMessage(userGuid);
            cart.BillingAddressSameAsShippingAddress = _billingAddressSameAsShippingAddress.Get(cart, userGuid);
            cart.TermsAndConditionsRequired = _ecommerceSettings.TermsAndConditionsRequired;
            cart.TermsAndConditionsAccepted = GetTermsAndConditionsAccepted(userGuid);
            return cart;
        }

        private void AssignPricingMethod(List<CartItemData> cartItems)
        {
            foreach (var item in cartItems)
            {
                item.Pricing = _productPricingMethod;
            }
        }

        private bool GetTermsAndConditionsAccepted(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<bool>(CartManager.TermsAndConditionsAcceptedKey, userGuid);
        }

        private void AssignAvailability(List<CartItemData> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                var item = cartItem;
                item.CanBuyStatus = _cartItemAvailabilityService.CanBuy(item);
            }
        }

        private string GetGiftMessage(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentGiftMessageKey, userGuid);
        }


        private string GetOrderEmail(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentOrderEmailKey, userGuid);
        }

        private List<CartItemData> GetItems(Guid userGuid)
        {
            var cartItems = _session.QueryOver<CartItem>()
                .Where(item => item.UserGuid == userGuid)
                .Cacheable()
                .List().ToList();
            return cartItems.Select(item => item.GetCartItemData()).ToList();
        }

        private Guid GetCartGuid(Guid userGuid)
        {
            Guid value = _cartSessionManager.GetSessionValue(CartManager.CurrentCartGuid, userGuid, Guid.Empty);
            if (value == Guid.Empty)
            {
                value = _cartGuidResetter.ResetCartGuid(userGuid);
            }
            return value;
        }


        private void DeleteNullProducts(IEnumerable<CartItemData> items)
        {
            foreach (CartItemData cartItem in items.Where(x => x.Item == null || x.Item.Product == null))
            {
                CartItem item = _session.Get<CartItem>(cartItem.Id);
                _session.Transact(session => _session.Delete(item));
            }
        }
    }
}