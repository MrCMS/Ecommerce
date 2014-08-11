using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class GetTaxRateOptionsTests : InMemoryDatabaseTest
    {
        private readonly GetTaxRateOptions _getTaxRateOptions;

        public GetTaxRateOptionsTests()
        {
            _getTaxRateOptions = new GetTaxRateOptions(Session);
        }

        [Fact]
        public void Get_ShouldHaveRecordsForTheSavedTaxRates()
        {
            List<TaxRate> taxRates =
                Enumerable.Range(1, 3).Select(i => new TaxRate { Percentage = i, Name = "Rate " + i }).ToList();
            Session.Transact(session => taxRates.ForEach(rate => session.Save(rate)));

            List<SelectListItem> allRates = _getTaxRateOptions.GetOptions();

            taxRates.ForEach(rate =>
            {
                allRates[rate.Id].Value.Should().Be(rate.Id.ToString());
                allRates[rate.Id].Text.Should().Be(rate.Name.ToString());
            });
            allRates[0].Text.Should().Be("Default Tax Rate");
        }
    }
}