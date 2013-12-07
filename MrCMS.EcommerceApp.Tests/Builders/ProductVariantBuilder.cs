using MrCMS.EcommerceApp.Tests.Entities.Products.ProductVariantTests;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ProductVariantBuilder
    {
        private bool? _inStockStatus;
        private int _stock;
        private TrackingPolicy _trackingPolicy = TrackingPolicy.DontTrack;

        public ProductVariantBuilder IsOutOfStock()
        {
            _inStockStatus = false;
            return this;
        }

        public ProductVariantBuilder WithStockRemaining(int stock)
        {
            _trackingPolicy = TrackingPolicy.Track;
            _stock = stock;
            return this;
        }

        public ProductVariant Build()
        {
            return new TestableProductVariant(_inStockStatus) { StockRemaining = _stock, TrackingPolicy = _trackingPolicy };
        }
    }
}