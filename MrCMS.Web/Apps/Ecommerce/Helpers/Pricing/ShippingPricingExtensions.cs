using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Pricing
{
    public static class ShippingPricingExtensions
    {
        public static decimal ShippingPriceIncludingTax(this decimal amount, decimal taxRatePercentage)
        {
            return GetService().GetPriceIncludingTax(amount, taxRatePercentage);
        }

        public static decimal ShippingPriceExcludingTax(this decimal amount, decimal taxRatePercentage)
        {
            return GetService().GetPriceExcludingTax(amount, taxRatePercentage);
        }

        public static decimal ShippingTax(this decimal amount, decimal taxRatePercentage)
        {
            return GetService().GetTax(amount, taxRatePercentage);
        }

        public static decimal ShippingPriceIncludingTax(this decimal? amount, decimal taxRatePercentage)
        {
            return GetService().GetPriceIncludingTax(amount, taxRatePercentage);
        }

        public static decimal ShippingPriceExcludingTax(this decimal? amount, decimal taxRatePercentage)
        {
            return GetService().GetPriceExcludingTax(amount, taxRatePercentage);
        }

        public static decimal ShippingTax(this decimal? amount, decimal taxRatePercentage)
        {
            return GetService().GetTax(amount, taxRatePercentage);
        }

        private static IShippingPricingService GetService()
        {
            return MrCMSApplication.Get<IShippingPricingService>();
        }
    }
}