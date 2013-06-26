using System;
using System.ComponentModel;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using System.Web.Mvc;
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
            get
            {
                return Math.Round(MrCMSApplication.Get<TaxSettings>().ShippingRateIncludesTax
                                      ? BaseAmount / ((TaxRatePercentage + 100) / 100)
                                      : BaseAmount, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual decimal Amount
        {
            get
            {
                return Math.Round(MrCMSApplication.Get<TaxSettings>().ShippingRateIncludesTax
                                      ? BaseAmount
                                      : TaxRate != null
                                            ? BaseAmount * (TaxRate.Multiplier)
                                            : BaseAmount, 2, MidpointRounding.AwayFromZero);
            }
        }

        [DisplayName("Shipping Method")]
        public virtual ShippingMethod ShippingMethod { get; set; }

         [Required]
        public virtual Country Country { get; set; }

        public virtual bool CanBeUsed(CartModel model)
        {
            return GetPrice(model) != null;
        }

        public virtual decimal? GetPrice(CartModel model)
        {
            switch (ShippingCriteria)
            {
                case ShippingCriteria.ByWeight:
                    return GetPriceByWeight(model);
                case ShippingCriteria.ByCartTotal:
                    return GetPriceByCartTotal(model);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private decimal? GetPriceByCartTotal(CartModel model)
        {
            return GetPrice(model.TotalPreShipping);
        }

        private decimal? GetPriceByWeight(CartModel model)
        {
            return GetPrice(model.Weight);
        }

        private decimal? GetPrice(decimal value)
        {
            if (UpperBound.HasValue && value > UpperBound)
                return null;
            if (value < LowerBound)
                return null;
            return Amount;
        }

        public virtual decimal? GetTax(CartModel model)
        {
            var price = GetPrice(model);
            return price == null ? (decimal?) null : Tax;
        }

        public virtual decimal Tax
        {
            get { return Amount - AmountPreTax; }
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
                       ? string.Format("{0:C} to {1:C}", LowerBound.ToString("#.##"), UpperBound.Value.ToString("#.##"))
                       : string.Format("{0:C} or greater", LowerBound.ToString("#.##"));
        }

        private string GetCartWeightValue()
        {
            return UpperBound.HasValue
                       ? string.Format("{0}kg to {1}kg", LowerBound.ToString("#.##"), UpperBound.Value.ToString("#.##"))
                       : string.Format("{0}kg or greater", LowerBound.ToString("#.##"));
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