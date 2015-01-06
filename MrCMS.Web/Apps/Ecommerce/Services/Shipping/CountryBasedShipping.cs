using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Helpers.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class CountryBasedShipping : IShippingMethod
    {
        private readonly CountryBasedShippingSettings _countryBasedShippingSettings;
        private readonly IGetDefaultTaxRate _getDefaultTaxRate;
        private readonly ISession _session;

        public CountryBasedShipping(CountryBasedShippingSettings countryBasedShippingSettings, ISession session,
            IGetDefaultTaxRate getDefaultTaxRate)
        {
            _countryBasedShippingSettings = countryBasedShippingSettings;
            _session = session;
            _getDefaultTaxRate = getDefaultTaxRate;
        }

        public string Name
        {
            get { return "Country based shipping"; }
        }

        public string DisplayName
        {
            get { return _countryBasedShippingSettings.DisplayName; }
        }

        public string Description
        {
            get { return _countryBasedShippingSettings.Description; }
        }

        public string TypeName
        {
            get { return GetType().FullName; }
        }

        public bool CanBeUsed(CartModel cart)
        {
            if (!CanPotentiallyBeUsed(cart))
                return false;
            if (cart.ShippingAddress == null)
                return false;
            return GetBestAvailableCalculation(cart) != null;
        }

        public bool CanPotentiallyBeUsed(CartModel cart)
        {
            if (cart == null || cart.Items.Any(item => !item.IsAbleToUseShippingMethod(this)))
                return false;
            CountryBasedShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation != null;
        }

        public decimal GetShippingTotal(CartModel cart)
        {
            CountryBasedShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? decimal.Zero
                : calculation.Amount(TaxRatePercentage);
        }

        public decimal GetShippingTax(CartModel cart)
        {
            CountryBasedShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? decimal.Zero
                : calculation.Tax(TaxRatePercentage);
        }

        public decimal TaxRatePercentage
        {
            get
            {
                int? taxRateId = _countryBasedShippingSettings.TaxRateId;
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

        public string ConfigureAction
        {
            get { return "Configure"; }
        }

        public string ConfigureController
        {
            get { return "CountryBasedShipping"; }
        }

        private CountryBasedShippingCalculation GetBestAvailableCalculation(CartModel cart)
        {
            HashSet<CountryBasedShippingCalculation> calculations =
                _session.QueryOver<CountryBasedShippingCalculation>().Cacheable().List().ToHashSet();
            calculations = calculations.FilterToAvailable(cart);
            HashSet<CountryBasedShippingCalculation> potentialCalculations =
                calculations.FindAll(calculation => calculation.CanBeUsed(cart));
            return potentialCalculations.OrderBy(calculation => calculation.Amount(TaxRatePercentage)).FirstOrDefault();
        }
    }
}