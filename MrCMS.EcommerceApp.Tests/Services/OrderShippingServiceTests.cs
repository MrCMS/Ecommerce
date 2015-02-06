using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class BulkShippingServiceTests : InMemoryDatabaseTest
    {
        private readonly IBulkShippingUpdateService _bulkShippingUpdateService;
        private readonly IBulkShippingUpdateValidationService _bulkShippingUpdateValidationService;
        private readonly BulkShippingService _orderShippingService;

        public BulkShippingServiceTests()
        {
            _bulkShippingUpdateValidationService = A.Fake<IBulkShippingUpdateValidationService>();
            _bulkShippingUpdateService = A.Fake<IBulkShippingUpdateService>();
            _orderShippingService = new BulkShippingService(_bulkShippingUpdateValidationService,
                _bulkShippingUpdateService);
        }

        [Fact]
        public void BulkShippingService_Update_ShouldNotBeNull()
        {
            Dictionary<string, List<string>> result = _orderShippingService.Update(GetDefaultStream(), false);

            result.Should().NotBeNull();
        }

        [Fact]
        public void BulkShippingService_Update_ShouldReturnDictionary()
        {
            Dictionary<string, List<string>> result = _orderShippingService.Update(GetDefaultStream(), false);

            result.Should().HaveCount(1);
        }

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}