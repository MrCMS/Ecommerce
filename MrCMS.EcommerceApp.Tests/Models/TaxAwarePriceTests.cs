using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models
{
    public class TaxAwarePriceTests
    {
        [Fact]
        public void TaxAwarePrice_PriceExcludingTax_IsNullWhenPassedValueIsNull()
        {
            var taxAwarePrice = new TaxAwarePrice(null, new TaxRate());

            taxAwarePrice.PriceExcludingTax.Should().Be(null);
        }

        [Fact]
        public void TaxAwarePrice_PriceIncludingTax_IsNullWhenPassedValueIsNull()
        {
            var taxAwarePrice = new TaxAwarePrice(null, new TaxRate());

            taxAwarePrice.PriceIncludingTax.Should().Be(null);
        }
    }
}