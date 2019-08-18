using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Orders.Sync
{
    public class SetTaxesTests : InMemoryDatabaseTest
    {
        private readonly AmazonSyncSettings _amazonSyncSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ISetTaxDetails _setTaxDetails;
        private readonly ISetTaxes _setTaxes;

        public SetTaxesTests()
        {
            _amazonSyncSettings = new AmazonSyncSettings() { UseDefaultTaxRateForShippingTax = true, TryCalculateVat = true };
            _taxSettings = new TaxSettings() { TaxesEnabled = true, ShippingRateTaxesEnabled = true };
            _setTaxDetails = A.Fake<ISetTaxDetails>();
            _setTaxes = new SetTaxes(_amazonSyncSettings, _taxSettings, _setTaxDetails);
        }

        [Fact]
        public void SetTaxesService_SetTax_ShouldNotProceedWithSettingTaxIfTaxValueIsProvided()
        {
            var order = new Order() { Total = 10 };

            _setTaxes.SetTax(ref order, 5);

            A.CallTo(() => _setTaxDetails.SetOrderLinesTaxes(ref order)).MustNotHaveHappened();
            A.CallTo(() => _setTaxDetails.SetShippingTaxes(ref order)).MustNotHaveHappened();
        }

        [Fact]
        public void SetTaxesService_SetTax_ShouldSetTaxIfTaxValueIsProvided()
        {
            var order = new Order() { Total = 10 };

            _setTaxes.SetTax(ref order, 5);

            order.Tax.Should().Be(5);
        }

        [Fact]
        public void SetTaxesService_SetTax_ShouldCallSetOrderLinesTaxes()
        {
            var order = new Order() {Total = 10};

            _setTaxes.SetTax(ref order, 0);

            A.CallTo(() => _setTaxDetails.SetOrderLinesTaxes(ref order)).MustHaveHappened();
        }

        [Fact]
        public void SetTaxesService_SetTax_ShouldCallSetShippingTaxes()
        {
            var order = new Order() { Total = 10 };

            _setTaxes.SetTax(ref order,0);

            A.CallTo(() => _setTaxDetails.SetShippingTaxes(ref order)).MustHaveHappened();
        }
    }
}