using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class TaxRateManagerTests : InMemoryDatabaseTest
    {
        [Fact]
        public void TaxRateManager_GetAll_ReturnsAllSavedTaxRates()
        {
            TaxRateManager taxRateManager = GetTaxRateManager();
            List<TaxRate> taxRates = Enumerable.Range(1, 10).Select(i => new TaxRate { Percentage = i }).ToList();
            Session.Transact(session => taxRates.ForEach(rate => session.Save(rate)));

            IList<TaxRate> allRates = taxRateManager.GetAll();

            allRates.Should().BeEquivalentTo(taxRates);
        }

        [Fact]
        public void TaxRateManager_Add_SavesThePassedTaxRateToSession()
        {
            TaxRateManager taxRateManager = GetTaxRateManager();
            
            taxRateManager.Add(new TaxRate());

            Session.QueryOver<TaxRate>().RowCount().Should().Be(1);
        }

        [Fact]
        public void TaxRateManager_Update_UpdatesTheExistingTaxRate()
        {
            TaxRateManager taxRateManager = GetTaxRateManager();
            var taxRate = new TaxRate();
            Session.Transact(session => session.Save(taxRate));
            taxRate.Name = "Updated";

            taxRateManager.Update(taxRate);

            Session.Evict(taxRate);
            Session.Get<TaxRate>(1).Name.Should().Be("Updated");
        }

        [Fact]
        public void TaxRateManager_Delete_RemovesTheTaxRateFromTheSession()
        {
            TaxRateManager taxRateManager = GetTaxRateManager();
            var taxRate = new TaxRate();
            Session.Transact(session => session.Save(taxRate));

            taxRateManager.Delete(taxRate);

            Session.QueryOver<TaxRate>().RowCount().Should().Be(0);
        }

        private TaxRateManager GetTaxRateManager()
        {
            return new TaxRateManager(Session);
        }
    }
}