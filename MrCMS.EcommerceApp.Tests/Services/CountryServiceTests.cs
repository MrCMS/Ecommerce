using System.Linq;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class CountryServiceTests : InMemoryDatabaseTest
    {
        private CountryService _countryService;

        public CountryServiceTests()
        {
            _countryService = new CountryService(Session);
        }

        [Fact]
        public void CountryService_GetAllCountries_ReturnsAllPersistedCountries()
        {
            var countries = Enumerable.Range(1, 10).Select(i => new Country {DisplayOrder = i, Name = "Country " + i}).ToList();
            Session.Transact(session => countries.ForEach(country => session.Save(country)));

            var allCountries = _countryService.GetAllCountries();

            allCountries.Should().BeEquivalentTo(countries);
        }

        [Fact]
        public void CountryService_GetCountriesToAdd_ReturnsAllCountriesWhenNoneAreAdded()
        {
            var allCountries = _countryService.GetCountriesToAdd();

            allCountries.Should().HaveCount(249);
        }

        [Fact]
        public void CountryService_GetCountriesToAdd_IfCountriesAreAddedToSessionTheyShouldBeRemovedFromList()
        {
            Session.Transact(session => session.Save(new Country{Name="UK", ISOTwoLetterCode = "GB"}));
            Session.Transact(session => session.Save(new Country{Name="US", ISOTwoLetterCode = "US"}));
            Session.Transact(session => session.Save(new Country{Name="France", ISOTwoLetterCode = "FR"}));

            var allCountries = _countryService.GetCountriesToAdd();

            allCountries.Should().HaveCount(246);
            allCountries.Select(item => item.Value).Should().NotContain("GB");
            allCountries.Select(item => item.Value).Should().NotContain("US");
            allCountries.Select(item => item.Value).Should().NotContain("FR");
        }

        [Fact]
        public void CountryService_AddCountry_ShouldAddCountryToSession()
        {
            _countryService.AddCountry("GB");

            Session.QueryOver<Country>().List().Should().HaveCount(1);
        }

        [Fact]
        public void CountryService_AddCountry_ShouldSetNameFromData()
        {
            _countryService.AddCountry("GB");

            var country = Session.QueryOver<Country>().List().First();

            country.Name.Should().Be("UNITED KINGDOM");
        }

        [Fact]
        public void CountryService_Save_ShouldUpdateCountry()
        {
            var country = new Country {Name = "UK", ISOTwoLetterCode = "GB"};
            Session.Transact(session => session.Save(country));
            country.Name = "United Kingdom";

            _countryService.Save(country);
            Session.Evict(country);

            Session.QueryOver<Country>().SingleOrDefault().Name.Should().Be("United Kingdom");
        }

        [Fact]
        public void CountryService_Delete_ShouldRemoveCountryFromSession()
        {
            var country = new Country {Name = "UK", ISOTwoLetterCode = "GB"};
            Session.Transact(session => session.Save(country));

            _countryService.Delete(country);
            Session.Evict(country);

            Session.QueryOver<Country>().RowCount().Should().Be(0);
        }
    }
}