using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ProductVariantBuilder : IEntityBuilder<ProductVariant>
    {
        private decimal _basePrice;
        private bool _hasRestrictedShipping;
        private string[] _restrictedTo = new string[0];
        private int _stockRemaining = 100;
        private decimal _taxRatePercentage;
        private TrackingPolicy _trackingPolicy = TrackingPolicy.Track;

        public ProductVariant Build()
        {
            return new ProductVariant
            {
                TrackingPolicy = _trackingPolicy,
                StockRemaining = _stockRemaining,
                HasRestrictedShipping = _hasRestrictedShipping,
                RestrictedTo = _restrictedTo.ToHashSet(),
                BasePrice = _basePrice,
                TaxRate = new TaxRate {Percentage = _taxRatePercentage}
            };
        }

        public ProductVariantBuilder DoNotTrackStock()
        {
            _trackingPolicy = TrackingPolicy.DontTrack;
            return this;
        }

        public ProductVariantBuilder WithBasePrice(decimal basePrice)
        {
            _basePrice = basePrice;
            return this;
        }

        public ProductVariantBuilder WithTaxPercentage(decimal taxRatePercentage)
        {
            _taxRatePercentage = taxRatePercentage;
            return this;
        }

        public ProductVariantBuilder StockRemaining(int stockRemaining)
        {
            _stockRemaining = stockRemaining;
            return this;
        }

        public ProductVariantBuilder WithUnrestrictedShipping()
        {
            _hasRestrictedShipping = false;
            return this;
        }

        public ProductVariantBuilder WithRestrictedShipping(params string[] restrictedTo)
        {
            _hasRestrictedShipping = true;
            _restrictedTo = restrictedTo;
            return this;
        }
    }
}