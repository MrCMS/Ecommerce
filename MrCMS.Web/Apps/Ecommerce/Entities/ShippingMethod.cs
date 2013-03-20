using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class ShippingMethod : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }

    public class ShippingRateComputationMethod
    {
        public virtual ShippingMethod ShippingMethod { get; set; }

        public virtual bool WeightSpecific { get; set; }
        public virtual decimal WeightFrom { get; set; }
        public virtual decimal WeightTo { get; set; }

        public virtual bool IsPriceRange { get; set; }
        public virtual decimal BasePrice { get; set; }
        public virtual decimal? PriceTo { get; set; }

        public virtual bool IsValid(IShippingCalculationRequest request)
        {
            throw new NotImplementedException();
        }

        public virtual decimal? GetPrice(IShippingCalculationRequest request)
        {
            throw new NotImplementedException();
            //if (IsValid(request.Weight))
            //{
            //    if (PriceTo.HasValue)
            //    {
            //        var difference = WeightTo - WeightFrom;
            //        var amountFrom = request.Weight - WeightFrom;
            //        var distanceBetweenWeights = amountFrom / difference;
            //        var priceDifference = PriceTo - BasePrice;
            //        return Math.Round((BasePrice + priceDifference * distanceBetweenWeights).GetValueOrDefault(), 2,
            //                          MidpointRounding.AwayFromZero);
            //    }
            //    else
            //        return BasePrice;
            //}
            //return null;
        }
    }
}