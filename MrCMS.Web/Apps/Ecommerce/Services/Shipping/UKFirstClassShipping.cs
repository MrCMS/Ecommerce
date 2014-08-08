using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;
using MrCMS.Web.Apps.Ecommerce.Settings.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class UKFirstClassShipping : IShippingMethod
    {
        private readonly ISession _session;
        private readonly UKFirstClassShippingSettings _ukFirstClassShippingSettings;

        public UKFirstClassShipping(ISession session, UKFirstClassShippingSettings ukFirstClassShippingSettings)
        {
            _session = session;
            _ukFirstClassShippingSettings = ukFirstClassShippingSettings;
        }

        public bool CanBeUsed(CartModel cart)
        {
            if (cart == null || cart.ShippingAddress == null || cart.ShippingAddress.Country == null ||
                cart.ShippingAddress.Country.ISOTwoLetterCode != "GB")
                return false;
            return GetBestAvailableCalculation(cart) != null;
        }

        public ShippingAmount GetShippingTotal(CartModel cart)
        {
            UKFirstClassShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? ShippingAmount.NoneAvailable
                : ShippingAmount.Total(calculation.Amount(TaxRatePercentage));
        }

        public ShippingAmount GetShippingTax(CartModel cart)
        {
            UKFirstClassShippingCalculation calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? ShippingAmount.NoneAvailable
                : ShippingAmount.Total(calculation.Tax(TaxRatePercentage));
        }

        public string Name
        {
            get { return _ukFirstClassShippingSettings.Name; }
        }

        public string Description
        {
            get { return _ukFirstClassShippingSettings.Description; }
        }

        public decimal TaxRatePercentage
        {
            get
            {
                var taxRateId = _ukFirstClassShippingSettings.TaxRateId;
                var taxRate = taxRateId.HasValue
                    ? _session.Get<TaxRate>(taxRateId)
                    : null;
                return taxRate != null
                    ? taxRate.Percentage
                    : decimal.Zero;
            }
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