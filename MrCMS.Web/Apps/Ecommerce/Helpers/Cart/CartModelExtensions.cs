using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Cart
{
    public static class CartModelExtensions
    {
        public static string CartPageShipping(this CartModel cart)
        {
            switch (cart.ShippingStatus)
            {
                case CartShippingStatus.ShippingNotRequired:
                    return "Not required";
                case CartShippingStatus.CannotShip:
                    return "Cannot ship";
                case CartShippingStatus.ShippingNotSet:
                    var shippingAmounts =
                        cart.AvailableShippingMethods.Select(method => method.GetShippingTotal(cart))
                            .OrderBy(amount => amount.Value);
                    return string.Format("From {0}", shippingAmounts.First().Value.ToCurrencyFormat());
                case CartShippingStatus.ShippingSet:
                    return cart.ShippingMethod.GetShippingTotal(cart).Value.ToCurrencyFormat();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string CartPageTotal(this CartModel cart)
        {
            switch (cart.ShippingStatus)
            {
                case CartShippingStatus.ShippingSet:
                case CartShippingStatus.ShippingNotRequired:
                    return cart.Total.ToCurrencyFormat();
                case CartShippingStatus.CannotShip:
                    return "Cannot complete order";
                case CartShippingStatus.ShippingNotSet:
                    var shippingAmounts =
                        cart.AvailableShippingMethods.Select(method => method.GetShippingTotal(cart))
                            .OrderBy(amount => amount.Value);
                    var value = shippingAmounts.First().Value;
                    return string.Format("From {0}", (cart.TotalPreShipping + value).ToCurrencyFormat());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public class ShippingMethodSettings : SiteSettingsBase
    {
        public ShippingMethodSettings()
        {
            EnabledMethods = new Dictionary<string, bool>();
        }

        public Dictionary<string, bool> EnabledMethods { get; set; }

        public HashSet<Type> GetEnabledMethods()
        {
            return EnabledMethods.Keys.Where(s => EnabledMethods[s]).Select(TypeHelper.GetTypeByName).ToHashSet();
        }
    }

    public interface IShippingMethodUIService
    {
        HashSet<IShippingMethod> GetAvailableMethods();
    }

    public class ShippingMethodUIService : IShippingMethodUIService
    {
        private readonly HashSet<IShippingMethod> _availableShippingMethods;
        private readonly IKernel _kernel;

        public ShippingMethodUIService(IKernel kernel, ShippingMethodSettings shippingMethodSettings)
        {
            _kernel = kernel;
            _availableShippingMethods =
                shippingMethodSettings.GetEnabledMethods()
                    .Select(type => _kernel.Get(type) as IShippingMethod)
                    .ToHashSet();
        }

        public HashSet<IShippingMethod> GetAvailableMethods()
        {
            return _availableShippingMethods;
        }
    }

    public interface IGetCheapestShippingMethod
    {
        ShippingAmount Get(CartModel cart);
    }

    public class GetCheapestShippingMethod : IGetCheapestShippingMethod
    {
        private readonly IShippingMethodUIService _shippingMethodUIService;

        public GetCheapestShippingMethod(IShippingMethodUIService shippingMethodUIService)
        {
            _shippingMethodUIService = shippingMethodUIService;
        }

        public ShippingAmount Get(CartModel cart)
        {
            HashSet<IShippingMethod> availableMethods =
                _shippingMethodUIService.GetAvailableMethods()
                    .FindAll(method => method.GetShippingTotal(cart) != ShippingAmount.NoneAvailable);
            if (!availableMethods.Any())
                return ShippingAmount.NoneAvailable;
            return availableMethods.Select(method => method.GetShippingTotal(cart))
                .OrderBy(amount => amount.Value)
                .First();
        }
    }
}