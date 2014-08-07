using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Shipping
{
    public class ShippingCalculation : SiteEntity
    {
        [DisplayName("Shipping Criteria")]
        [Required]
        public virtual ShippingCriteria ShippingCriteria { get; set; }

        [DisplayName("Lower Bound")]
        [Required]
        [Remote("IsValidShippingCalculation", "ShippingCalculation",
            AdditionalFields = "Id,ShippingMethod.Id,Country.Id,ShippingCriteria,UpperBound")]
        public virtual decimal LowerBound { get; set; }

        [DisplayName("Upper Bound")]
        public virtual decimal? UpperBound { get; set; }

        [DisplayName("Amount")]
        [Required]
        public virtual decimal BaseAmount { get; set; }

        public virtual TaxRate TaxRate
        {
            get
            {
                return ShippingMethod != null
                           ? ShippingMethod.TaxRate
                           : null;
            }
        }

        public virtual decimal TaxRatePercentage
        {
            get
            {
                return TaxRate == null
                           ? 0
                           : TaxRate.Percentage;
            }
        }

        [DisplayName("Amount Pre Tax")]
        public virtual decimal AmountPreTax
        {
            get { return TaxAwareShippingRate.GetPriceExcludingTax(BaseAmount, TaxRate); }
        }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public virtual decimal Amount
        {
            get { return TaxAwareShippingRate.GetPriceIncludingTax(BaseAmount, TaxRate); }
        }

        [DisplayName("Shipping Method")]
        public virtual ShippingMethod ShippingMethod { get; set; }

        [Required]
        public virtual Country Country { get; set; }

        private HashSet<ProductVariant> _excludedProductVariants;
        public virtual HashSet<ProductVariant> ExcludedProductVariants
        {
            get
            {
                return
                    _excludedProductVariants =
                        _excludedProductVariants ??
                        new HashSet<ProductVariant>(
                            MrCMSApplication.Get<ISession>()
                                .QueryOver<ProductVariant>()
                                .JoinQueryOver<ShippingMethod>(p => p.RestrictedShippingMethods)
                                .Where(method => method.Id == ShippingMethod.Id).Cacheable().List());
            }
        }

        public virtual decimal Tax
        {
            get { return TaxAwareShippingRate.GetTax(BaseAmount, TaxRate); }
        }

        public virtual string Description
        {
            get
            {
                switch (ShippingCriteria)
                {
                    case ShippingCriteria.ByWeight:
                        return string.Format("Cart weight: {0}", GetCartWeightValue());
                    case ShippingCriteria.ByCartTotal:
                        return string.Format("Cart total: {0}", GetCartTotalValue());
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public virtual bool CanBeUsed(CartModel model)
        {
            if (ShippingMethod == null)
                return false;
            var excludedProductVariants = new HashSet<ProductVariant>(ExcludedProductVariants);
            if (model.Items.Any(item => excludedProductVariants.Contains(item.Item)))
                return false;
            if (model.ShippingAddress != null && model.ShippingAddress.Country != Country)
                return false;
            switch (ShippingCriteria)
            {
                case ShippingCriteria.ByWeight:
                    return IsValid(model.Weight);
                case ShippingCriteria.ByCartTotal:
                    return IsValid(model.ShippableCalculationTotal);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public virtual decimal? GetPrice(CartModel model)
        {
            return !CanBeUsed(model) ? (decimal?)null : Amount;
        }

        private bool IsValid(decimal value)
        {
            return value >= LowerBound && (!UpperBound.HasValue || value <= UpperBound);
        }

        public virtual decimal? GetTax(CartModel model)
        {
            return CanBeUsed(model) ? Tax : (decimal?)null;
        }

        private string GetCartTotalValue()
        {
            return UpperBound.HasValue
                       ? string.Format("{0} to {1}", LowerBound.ToString("0.00"), UpperBound.Value.ToString("0.00"))
                       : string.Format("{0} or greater", LowerBound.ToString("0.00"));
        }

        private string GetCartWeightValue()
        {
            return UpperBound.HasValue
                       ? string.Format("{0}kg to {1}kg", LowerBound.ToString("0.00"), UpperBound.Value.ToString("#.##"))
                       : string.Format("{0}kg or greater", LowerBound.ToString("0.00"));
        }
    }

    public enum ShippingCriteria
    {
        [Description("Based on cart weight")]
        ByWeight = 1,
        [Description("Based on cart total")]
        ByCartTotal = 2
    }
}