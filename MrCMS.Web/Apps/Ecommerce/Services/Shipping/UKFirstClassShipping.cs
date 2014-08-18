using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class UKFirstClassShipping : IShippingMethod
    {
        private readonly IGetDefaultTaxRate _getDefaultTaxRate;
        private readonly ISession _session;
        private readonly UKFirstClassShippingSettings _ukFirstClassShippingSettings;

        public UKFirstClassShipping(ISession session, UKFirstClassShippingSettings ukFirstClassShippingSettings,
            IGetDefaultTaxRate getDefaultTaxRate)
        {
            _session = session;
            _ukFirstClassShippingSettings = ukFirstClassShippingSettings;
            _getDefaultTaxRate = getDefaultTaxRate;
        }

        public string Name
        {
            get { return "UK First Class"; }
        }

        public string DisplayName
        {
            get { return _ukFirstClassShippingSettings.DisplayName; }
        }

        public string Description
        {
            get { return _ukFirstClassShippingSettings.Description; }
        }

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
            if (cart == null)
                return false;
            var calculation = GetBestAvailableCalculation(cart);
            return calculation != null;
        }

        public decimal GetShippingTotal(CartModel cart)
        {
            UKFirstClassShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? decimal.Zero
                : calculation.Amount(TaxRatePercentage);
        }

        public decimal GetShippingTax(CartModel cart)
        {
            UKFirstClassShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? decimal.Zero
                : calculation.Tax(TaxRatePercentage);
        }

        public decimal TaxRatePercentage
        {
            get
            {
                int? taxRateId = _ukFirstClassShippingSettings.TaxRateId;
                TaxRate taxRate = null;
                if (taxRateId.HasValue)
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
            get { return "UKFirstClassShipping"; }
        }

        public string ConfigureAction
        {
            get { return "Configure"; }
        }

        private UKFirstClassShippingCalculation GetBestAvailableCalculation(CartModel cart)
        {
            HashSet<UKFirstClassShippingCalculation> calculations =
                _session.QueryOver<UKFirstClassShippingCalculation>().Cacheable().List().ToHashSet();
            HashSet<UKFirstClassShippingCalculation> potentialCalculations =
                calculations.FindAll(calculation => calculation.CanBeUsed(cart));
            return potentialCalculations.OrderBy(calculation => calculation.Amount(TaxRatePercentage)).FirstOrDefault();
        }
    }
}