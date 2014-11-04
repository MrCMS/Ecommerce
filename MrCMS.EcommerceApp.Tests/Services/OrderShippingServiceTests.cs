using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class BulkShippingServiceTests : InMemoryDatabaseTest
    {
        private readonly BulkShippingService _orderShippingService;
        private readonly IBulkShippingUpdateValidationService _bulkShippingUpdateValidationService;
        private readonly IBulkShippingUpdateService _bulkShippingUpdateService;

        public BulkShippingServiceTests()
        {
            _bulkShippingUpdateValidationService = A.Fake<IBulkShippingUpdateValidationService>();
            _bulkShippingUpdateService = A.Fake<IBulkShippingUpdateService>();
            _orderShippingService = new BulkShippingService(_bulkShippingUpdateValidationService,_bulkShippingUpdateService);
        }

        [Fact]
        public void BulkShippingService_Update_ShouldNotBeNull()
        {
            var result = _orderShippingService.Update(GetDefaultStream());

            result.Should().NotBeNull();
        }

        [Fact]
        public void BulkShippingService_Update_ShouldReturnDictionary()
        {
            var result = _orderShippingService.Update(GetDefaultStream());

            result.Should().HaveCount(1);
        }

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}