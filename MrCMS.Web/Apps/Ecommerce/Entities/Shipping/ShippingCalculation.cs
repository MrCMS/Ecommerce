using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Shipping
{
    public class ShippingCalculation : SiteEntity
    {
        [DisplayName("Shipping Criteria")]
        [Required]
        public virtual ShippingCriteria ShippingCriteria { get; set; }
        [DisplayName("Lower Bound")]
        [Required]
        [Remote("IsValidShippingCalculation", "ShippingCalculation", AdditionalFields = "Id,ShippingMethod.Id,Country.Id,ShippingCriteria,UpperBound")]
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

        public virtual bool CanBeUsed(CartModel model)
        {
            if (ShippingMethod == null)
                return false;
            if (model.Items.Any(item => ExcludedProductVariants.Contains(item.Item)))
                return false;
            if (model.ShippingAddress != null && model.ShippingAddress.Country != Country)
                return false;
            switch (ShippingCriteria)
            {
                case ShippingCriteria.ByWeight:
                    return IsValid(model.Weight);
                case ShippingCriteria.ByCartTotal:
                    return IsValid(model.TotalPreShipping);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual IList<ProductVariant> ExcludedProductVariants
        {
            get { return ShippingMethod != null ? ShippingMethod.ExcludedProductVariants : new List<ProductVariant>(); }
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