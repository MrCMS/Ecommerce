using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Helpers.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class UKCourierShipping : IShippingMethod
    {
        private readonly IGetDefaultTaxRate _getDefaultTaxRate;
        private readonly ISession _session;
        private readonly UKCourierShippingSettings _ukCourierShippingSettings;

        public UKCourierShipping(ISession session, UKCourierShippingSettings ukCourierShippingSettings,
            IGetDefaultTaxRate getDefaultTaxRate)
        {
            _session = session;
            _ukCourierShippingSettings = ukCourierShippingSettings;
            _getDefaultTaxRate = getDefaultTaxRate;
        }

        public string Name
        {
            get { return "UK Courier Delivery"; }
        }

        public string DisplayName
        {
            get { return _ukCourierShippingSettings.DisplayName; }
        }

        public string Description
        {
            get { return _ukCourierShippingSettings.Description; }
        }

        public string TypeName { get { return GetType().FullName; } }

        public bool CanBeUsed(CartModel cart)
        {
            if (!CanPotentiallyBeUsed(cart))
                return false;
            if (cart.ShippingAddress == null)
                return false;
            return cart.ShippingAddress.CountryCode == "GB";
        }

        public bool CanPotentiallyBeUsed(CartModel cart)
        {
            if (cart == null || cart.Items.Any(item => !item.IsAbleToUseShippingMethod(this)))
                return false;
            var calculation = GetBestAvailableCalculation(cart);
            return calculation != null;
        }

        public decimal GetShippingTotal(CartModel cart)
        {
            UKCourierShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? decimal.Zero
                : calculation.Amount(TaxRatePercentage);
        }

        public decimal GetShippingTax(CartModel cart)
        {
            UKCourierShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? decimal.Zero
                : calculation.Tax(TaxRatePercentage);
        }


        public decimal TaxRatePercentage
        {
            get
            {
                int? taxRateId = _ukCourierShippingSettings.TaxRateId;
                TaxRate taxRate = null;
                if (taxRateId.GetValueOrDefault() > 0)
                {
                    taxRate = _session.Get<TaxRate>(taxRateId);
                }
                if (taxRate == null)
                {
                    taxRate = _getDefaultTaxRate.Get();
                }
                return taxRate != null
                    ? taxRate.Percentage
                    : decimal.Zero;
            }
        }

        public string ConfigureController
        {
            get { return "UKCourierShipping"; }
        }

        public string ConfigureAction
        {
            get { return "Configure"; }
        }

        private UKCourierShippingCalculation GetBestAvailableCalculation(CartModel cart)
        {
            HashSet<UKCourierShippingCalculation> calculations =
                _session.QueryOver<UKCourierShippingCalculation>().Cacheable().List().ToHashSet();
            calculations = calculations.FilterToAvailable(cart);
            HashSet<UKCourierShippingCalculation> potentialCalculations =
                calculations.FindAll(calculation => calculation.CanBeUsed(cart));
            return potentialCalculations.OrderBy(calculation => calculation.Amount(TaxRatePercentage)).FirstOrDefault();
        }
    }
}