﻿using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.EcommerceApp.Tests.Helpers;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Orders.Sync
{
    public class SetTaxDetailsTests : InMemoryDatabaseTest
    {
        private readonly AmazonSyncSettings _amazonSyncSettings;

        private readonly IGetProductVariantTaxRatePercentage _getProductVariantTaxRatePercentage =
            A.Fake<IGetProductVariantTaxRatePercentage>();

        private readonly ISetTaxDetails _setTaxes;
        private readonly ITaxRateManager _taxRateManager;
        private readonly TaxSettings _taxSettings;
        private readonly IProductVariantService _productVariantService;

        public SetTaxDetailsTests()
        {
            _amazonSyncSettings = new AmazonSyncSettings
            {
                UseDefaultTaxRateForShippingTax = true,
                TryCalculateVat = true
            };
            _taxSettings = new TaxSettings {TaxesEnabled = true, ShippingRateTaxesEnabled = true};
            _taxRateManager = A.Fake<ITaxRateManager>();
            _productVariantService = A.Fake<IProductVariantService>();
            _setTaxes = new SetTaxDetails(_amazonSyncSettings, _taxSettings, Session, _taxRateManager,
                _productVariantService, _getProductVariantTaxRatePercentage);
        }

        //[Fact]
        //public void SetTaxDetails_SetOrderLinesTaxes_ShouldCallGetDefaultRate()
        //{
        //    var orderLine = new OrderLine() { UnitPrice = 1, Price = 2, Quantity = 2 };
        //    var order = new Order() { Total = 10, ShippingTotal = 1, OrderLines = new List<OrderLine>() { orderLine } };

        //    _setTaxes.SetOrderLinesTaxes(ref order);

        //    A.CallTo(() => _taxRateManager.GetRateForOrderLine(orderLine)).MustHaveHappened();
        //}

        //[Fact]
        //public void SetTaxDetails_SetOrderLinesTaxes_ShouldSetProperties()
        //{
        //    Kernel.Rebind<TaxSettings>()
        //        .ToConstant(new TaxSettings {TaxesEnabled = true, LoadedPricesIncludeTax = true});
        //    var orderLine = new OrderLine() { UnitPrice = 6, Price = 12, Quantity = 2 };
        //    var order = new Order() { Total = 12, ShippingTotal = 0m, OrderLines = new List<OrderLine>() { orderLine } };

        //    var taxRate = new TaxRate() {Percentage = 20};

        //    A.CallTo(() => _taxRateManager.GetRateForOrderLine(orderLine)).Returns(taxRate);

        //    _setTaxes.SetOrderLinesTaxes(ref order);

        //    order.Tax.Should().Be(2);//decimal.Parse("0,22"));
        //    order.Total.Should().Be(12);//.Be(decimal.Parse("2,22"));

        //    order.OrderLines.First().UnitPrice.Should().Be(6); //.Be(decimal.Parse("1,1"));
        //    order.OrderLines.First().UnitPricePreTax.Should().Be(5);
        //    order.OrderLines.First().Price.Should().Be(12); //.Be(decimal.Parse("2,2"));
        //    order.OrderLines.First().PricePreTax.Should().Be(10);
        //    order.OrderLines.First().Tax.Should().Be(2);//.Be(decimal.Parse("0,22"));
        //    order.OrderLines.First().TaxRate.Should().Be(20);
        //}

        //[Fact]
        //public void SetTaxDetails_SetOrderLinesTaxes_TakesIntoAccountRounding()
        //{
        //    Kernel.SetTaxSettings(true, true);
        //    var orderLine = new OrderLine() { UnitPrice = 6.51m, Price = 13.02m, Quantity = 2 };
        //    var order = new Order() { Total = 13.02m, OrderLines = new List<OrderLine>() { orderLine } };

        //    var taxRate = new TaxRate() { Percentage = 20 };

        //    A.CallTo(() => _taxRateManager.GetRateForOrderLine(orderLine)).Returns(taxRate);

        //    _setTaxes.SetOrderLinesTaxes(ref order);

        //    order.Tax.Should().Be(2.18m);//decimal.Parse("0,22"));
        //    order.Total.Should().Be(13.02m);//.Be(decimal.Parse("2,22"));

        //    order.OrderLines.First().UnitPrice.Should().Be(6.51m); //.Be(decimal.Parse("1,1"));
        //    order.OrderLines.First().UnitPricePreTax.Should().Be(5.42m);
        //    order.OrderLines.First().Price.Should().Be(13.02m); //.Be(decimal.Parse("2,2"));
        //    order.OrderLines.First().PricePreTax.Should().Be(10.84m);
        //    order.OrderLines.First().Tax.Should().Be(2.18m);//.Be(decimal.Parse("0,22"));
        //    order.OrderLines.First().TaxRate.Should().Be(20);
        //}

        [Fact]
        public void SetTaxDetails_SetShippingTaxes_ShouldSetProperties()
        {
            Kernel.SetTaxSettings(true, true, true);
            var order = new Order {ShippingTotal = 3};

            var taxRate = new TaxRate {Percentage = 50};

            A.CallTo(() => _taxRateManager.GetDefaultRate()).Returns(taxRate);

            _setTaxes.SetShippingTaxes(ref order);

            order.ShippingTax.Should().Be(1);
            order.ShippingTaxPercentage.Should().Be(50);
            order.ShippingTotal.Should().Be(3);
        }

        [Fact]
        public void SetTaxDetails_SetShippingTaxes_ShouldNotSetPropertiesIfNoTaxRateIsFound()
        {
            var order = new Order {ShippingTotal = 3};

            A.CallTo(() => _taxRateManager.GetDefaultRate()).Returns(null);

            _setTaxes.SetShippingTaxes(ref order);

            order.ShippingTax.Should().Be(null);
            order.ShippingTaxPercentage.Should().Be(null);
        }
    }
}