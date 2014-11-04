using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class GetDefaultTaxRateTests : InMemoryDatabaseTest
    {
        private readonly GetDefaultTaxRate _getDefaultTaxRate;

        public GetDefaultTaxRateTests()
        {
            _getDefaultTaxRate = new GetDefaultTaxRate(Session);
        }

        [Fact]
        public void Get_ShouldReturnTaxRate()
        {
            var taxRate = new TaxRate {Percentage = 10, IsDefault = true, Name = "GLOBAL", Code = "GL"};

            Session.Transact(session => session.Save(taxRate));

            TaxRate result = _getDefaultTaxRate.Get();

            result.Should().NotBeNull();
            result.Should().Be(taxRate);
        }
    }
}