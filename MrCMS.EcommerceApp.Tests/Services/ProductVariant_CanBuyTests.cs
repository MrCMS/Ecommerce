using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using NHibernate;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ProductVariantAvailabilityServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void IfOutOfStockShouldReturnAnOutOfStockStatus()
        {
            ProductVariant productVariant = new ProductVariantBuilder().IsOutOfStock().Build();
            ProductVariantAvailabilityService service = new ProductVariantAvailabilityServiceBuilder(Session).Build();

            service.CanBuy(productVariant).Should().BeOfType<OutOfStock>();
        }

        [Fact]
        public void IfInStockButCannotPurchaseTheCartItemAmountShouldReturnAnUnableToOrderQuantityStatus()
        {
            ProductVariant variant = new ProductVariantBuilder().WithStockRemaining(1).Build();
            CartItem item = new CartItemBuilder().WithQuantity(2).WithItem(variant).Build();
            CartModel cartModel = new CartModelBuilder().WithItems(item).Build();
            ProductVariantAvailabilityService service =
                new ProductVariantAvailabilityServiceBuilder(Session).WithCart(cartModel).Build();

            service.CanBuy(variant).Should().BeOfType<CannotOrderQuantity>();
        }

        [Fact]
        public void IfAdditionalQuantityTakesAmountRequestedOverStockRemainingShouldReturnCannotOrderQuantity()
        {
            ProductVariant variant = new ProductVariantBuilder().WithStockRemaining(2).Build();
            CartItem item = new CartItemBuilder().WithQuantity(2).WithItem(variant).Build();
            CartModel cartModel = new CartModelBuilder().WithItems(item).Build();
            ProductVariantAvailabilityService service =
                new ProductVariantAvailabilityServiceBuilder(Session).WithCart(cartModel).Build();

            service.CanBuy(variant, 1).Should().BeOfType<CannotOrderQuantity>();
        }

        [Fact(Skip = "Refactoring")]
        public void IfOnlyAvailableShippingMethodIsExcludedShouldReturnNoShippingMethodIsAvailable()
        {
            throw new NotImplementedException();
            //var variant = new ProductVariant { TrackingPolicy = TrackingPolicy.Track, StockRemaining = 2 };
            //var item = new CartItemBuilder().WithQuantity(2).WithItem(variant).Build();
            //var cartModel = new CartModelBuilder().WithItems(item).Build();
            //variant.RestrictedShippingMethods = cartModel.AvailableShippingMethods;
            //Session.Transact(session => session.Save(variant));
            //var service = new ProductVariantAvailabilityServiceBuilder(Session).WithCart(cartModel).Build();

            //service.CanBuy(variant).Should().BeOfType<NoShippingMethodWouldBeAvailable>();
        }

        [Fact(Skip = "Refactoring")]
        public void IfInStockAndCartItemAmountIsWithinStockRemainingShouldReturnCanBuy()
        {
            ProductVariant variant = new ProductVariantBuilder().WithStockRemaining(2).Build();
            CartItem item = new CartItemBuilder().WithQuantity(2).WithItem(variant).Build();
            CartModel cartModel = new CartModelBuilder().WithItems(item).Build();
            ProductVariantAvailabilityService service =
                new ProductVariantAvailabilityServiceBuilder(Session).WithCart(cartModel).Build();

            service.CanBuy(variant).Should().BeOfType<CanBuy>();
        }
    }

    public class ProductVariantAvailabilityServiceBuilder
    {
        private readonly ISession _session;
        private CartModel _cart = new CartModel();

        public ProductVariantAvailabilityServiceBuilder(ISession session)
        {
            _session = session;
        }

        public ProductVariantAvailabilityServiceBuilder WithCart(CartModel cart)
        {
            _cart = cart;
            return this;
        }

        public ProductVariantAvailabilityService Build()
        {
            return new ProductVariantAvailabilityService(_cart, _session);
        }
    }
}