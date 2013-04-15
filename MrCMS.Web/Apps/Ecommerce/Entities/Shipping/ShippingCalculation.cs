using System;
using System.ComponentModel;
using System.Web;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Shipping
{
    public class ShippingCalculation : SiteEntity
    {
        public virtual string Name { get; set; }
        [DisplayName("Shipping Criteria")]
        public virtual ShippingCriteria ShippingCriteria { get; set; }
        [DisplayName("Lower Bound")]
        public virtual decimal LowerBound { get; set; }
        [DisplayName("Upper Bound")]
        public virtual decimal? UpperBound { get; set; }
        [DisplayName("Base Price")]
        public virtual decimal BasePrice { get; set; }
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
        [DisplayName("Price Pre Tax")]
        public virtual decimal PricePreTax
        {
            get
            {
                return Math.Round(MrCMSApplication.Get<TaxSettings>().ShippingRateIncludesTax
                                      ? BasePrice / ((TaxRatePercentage + 100) / 100)
                                      : BasePrice, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual decimal Price
        {
            get
            {
                return Math.Round(MrCMSApplication.Get<TaxSettings>().ShippingRateIncludesTax
                                      ? BasePrice
                                      : TaxRate != null
                                            ? BasePrice * (TaxRate.Multiplier)
                                            : BasePrice, 2, MidpointRounding.AwayFromZero);
            }
        }

        [DisplayName("Shipping Method")]
        public virtual ShippingMethod ShippingMethod { get; set; }

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
            return GetPrice(model.Total);
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
            return Price;
        }

        public virtual decimal? GetTax(CartModel model)
        {
            var price = GetPrice(model);
            return price == null ? (decimal?) null : Tax;
        }

        public virtual decimal Tax
        {
            get { return Price - PricePreTax; }
        }

        public virtual string Description
        {
            get
            {
                switch (ShippingCriteria)
                {
                    case ShippingCriteria.ByWeight:
                        return string.Format("Cart weight: {0}", GetCartWeightValue());
                        break;
                    case ShippingCriteria.ByCartTotal:
                        return string.Format("Cart total: {0}", GetCartTotalValue());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string GetCartTotalValue()
        {
            return UpperBound.HasValue
                       ? string.Format("{0:C} to {1:C}", LowerBound, UpperBound)
                       : string.Format("{0:C} or greater", LowerBound);
        }

        private string GetCartWeightValue()
        {
            return UpperBound.HasValue
                       ? string.Format("{0}kg to {1}kg", LowerBound, UpperBound)
                       : string.Format("{0}kg or greater", LowerBound);
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