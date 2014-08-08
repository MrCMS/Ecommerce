using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class UKCourierShipping : IShippingMethod
    {
        private readonly ISession _session;
        private readonly UKCourierShippingSettings _ukCourierShippingSettings;

        public UKCourierShipping(ISession session, UKCourierShippingSettings ukCourierShippingSettings)
        {
            _session = session;
            _ukCourierShippingSettings = ukCourierShippingSettings;
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
            var calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? ShippingAmount.NoneAvailable
                : ShippingAmount.Total(calculation.Amount(TaxRatePercentage));
        }

        public ShippingAmount GetShippingTax(CartModel cart)
        {
            var calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? ShippingAmount.NoneAvailable
                : ShippingAmount.Total(calculation.Tax(TaxRatePercentage));
        }

        public string Name
        {
            get { return _ukCourierShippingSettings.Name; }
        }

        public string Description
        {
            get { return _ukCourierShippingSettings.Description; }
        }

        public decimal TaxRatePercentage
        {
            get
            {
                var taxRateId = _ukCourierShippingSettings.TaxRateId;
                var taxRate = taxRateId.HasValue
                    ? _session.Get<TaxRate>(taxRateId)
                    : null;
                return taxRate != null
                    ? taxRate.Percentage
                    : decimal.Zero;
            }
        }

        private UKCourierShippingCalculation GetBestAvailableCalculation(CartModel cart)
        {
            HashSet<UKCourierShippingCalculation> calculations =
                _session.QueryOver<UKCourierShippingCalculation>().Cacheable().List().ToHashSet();
            HashSet<UKCourierShippingCalculation> potentialCalculations =
                calculations.FindAll(calculation => calculation.CanBeUsed(cart));
            return potentialCalculations.OrderBy(calculation => calculation.Amount(TaxRatePercentage)).FirstOrDefault();
        }
    }

    public class UKCourierShippingSettings : SiteSettingsBase
    {
        public UKCourierShippingSettings()
        {
            Name = "Courier Delivery";
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int? TaxRateId { get; set; }
    }
}