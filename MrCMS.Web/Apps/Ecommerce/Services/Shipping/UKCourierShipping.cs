using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class UKCourierShipping : IShippingMethod
    {
        private readonly ISession _session;
        private readonly UKCourierShippingSettings _ukCourierShippingSettings;
        private readonly IGetDefaultTaxRate _getDefaultTaxRate;

        public UKCourierShipping(ISession session, UKCourierShippingSettings ukCourierShippingSettings, IGetDefaultTaxRate getDefaultTaxRate)
        {
            _session = session;
            _ukCourierShippingSettings = ukCourierShippingSettings;
            _getDefaultTaxRate = getDefaultTaxRate;
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
                : ShippingAmount.Amount(calculation.Amount(TaxRatePercentage));
        }

        public ShippingAmount GetShippingTax(CartModel cart)
        {
            var calculation = GetBestAvailableCalculation(cart);
            return calculation == null
                ? ShippingAmount.NoneAvailable
                : ShippingAmount.Amount(calculation.Tax(TaxRatePercentage));
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

        public string ConfigureController { get { return "UKCourierShipping"; } }
        public string ConfigureAction { get { return "Configure"; } }

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

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? TaxRateId { get; set; }
    }
}