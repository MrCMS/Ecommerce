using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Orders.Sync
{
    public class SetTaxDetailsTests : InMemoryDatabaseTest
    {
        private readonly AmazonSyncSettings _amazonSyncSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ITaxRateManager _taxRateManager;
        private readonly ISetTaxDetails _setTaxes;

        public SetTaxDetailsTests()
        {
            _amazonSyncSettings = new AmazonSyncSettings() { UseDefaultTaxRateForShippingTax = true, TryCalculateVat = true };
            _taxSettings = new TaxSettings() { TaxesEnabled = true, ShippingRateTaxesEnabled = true };
            _taxRateManager = A.Fake<ITaxRateManager>();
            _setTaxes = new SetTaxDetails(_amazonSyncSettings, _taxSettings, _taxRateManager);
        }

        [Fact]
        public void SetTaxDetails_SetOrderLinesTaxes_ShouldCallGetDefaultRate()
        {
            var orderLine = new OrderLine() {UnitPrice = 1, Price = 2, Quantity = 2};
            var order = new Order() { Total = 10, ShippingTotal = 1, OrderLines = new List<OrderLine>(){orderLine}};

            _setTaxes.SetOrderLinesTaxes(ref order);

            A.CallTo(() => _taxRateManager.GetDefaultRate(orderLine)).MustHaveHappened();
        }

        [Fact]
        public void SetTaxDetails_SetOrderLinesTaxes_ShouldSetProperties()
        {
            var orderLine = new OrderLine() { UnitPrice = 1, Price = 2, Quantity = 2 };
            var order = new Order() { Total = 2, ShippingTotal = 1, OrderLines = new List<OrderLine>() { orderLine } };

            var taxRate = new TaxRate() {Percentage = 10};

            A.CallTo(() => _taxRateManager.GetDefaultRate(orderLine)).Returns(taxRate);

            _setTaxes.SetOrderLinesTaxes(ref order);

            order.Tax.Should().Be(decimal.Parse("0,22"));
            order.Total.Should().Be(decimal.Parse("2,22"));

            order.OrderLines.First().UnitPrice.Should().Be(decimal.Parse("1,1"));
            order.OrderLines.First().UnitPricePreTax.Should().Be(1);
            order.OrderLines.First().Price.Should().Be(decimal.Parse("2,2"));
            order.OrderLines.First().PricePreTax.Should().Be(2);
            order.OrderLines.First().Tax.Should().Be(decimal.Parse("0,22"));
            order.OrderLines.First().TaxRate.Should().Be(10);
        }

        [Fact]
        public void SetTaxDetails_SetShippingTaxes_ShouldSetProperties()
        {
            var order = new Order() { Total = 2, ShippingTotal = 2};

            var taxRate = new TaxRate() { Percentage = 50 };

            A.CallTo(() => _taxRateManager.GetDefaultRate()).Returns(taxRate);

            _setTaxes.SetShippingTaxes(ref order);

            order.ShippingTax.Should().Be(1);
            order.ShippingTaxPercentage.Should().Be(50);
            order.Total.Should().Be(3);
        }

        [Fact]
        public void SetTaxDetails_SetShippingTaxes_ShouldNotSetPropertiesIfNoTaxRateIsFound()
        {
            var order = new Order() { Total = 2, ShippingTotal = 2 };

            A.CallTo(() => _taxRateManager.GetDefaultRate()).Returns(null);

            _setTaxes.SetShippingTaxes(ref order);

            order.ShippingTax.Should().Be(null);
            order.ShippingTaxPercentage.Should().Be(null);
        }
    }
}