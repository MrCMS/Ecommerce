using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class RegionServiceTests : InMemoryDatabaseTest
    {
        private RegionService _regionService;

        public RegionServiceTests()
        {
            _regionService = new RegionService(Session);
        }

        [Fact]
        public void RegionService_Add_ShouldPersistToSession()
        {
            _regionService.Add(new Region());

            Session.QueryOver<Region>().RowCount().Should().Be(1);
        }

        [Fact]
        public void RegionService_Update_ShouldUpdateInSession()
        {
            var region = new Region();
            Session.Transact(session => session.Save(region));
            region.Name = "updated";

            _regionService.Update(region);
            Session.Evict(region);

            Session.Get<Region>(1).Name.Should().Be("updated");
        }

        [Fact]
        public void RegionService_Delete_ShouldRemoveRegionFromTheSession()
        {
            var region = new Region();
            Session.Transact(session => session.Save(region));

            _regionService.Delete(region);

            Session.QueryOver<Region>().RowCount().Should().Be(0);
        }
    }
}