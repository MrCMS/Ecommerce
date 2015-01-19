using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.BulkShippingUpdate
{
    public class BulkShippingUpdateServiceTests : InMemoryDatabaseTest
    {
        private readonly BulkShippingUpdateService _bulkShippingUpdateService;
        private readonly IOrderAdminService _orderAdminService;

        public BulkShippingUpdateServiceTests()
        {
            _orderAdminService = A.Fake<IOrderAdminService>();
            _bulkShippingUpdateService = new BulkShippingUpdateService(_orderAdminService,
                A.Fake<IShippingMethodAdminService>(), Session);
        }

        [Fact]
        public void
            BulkShippingUpdateService_BulkShippingUpdateFromDTOs_ShouldReturnZeroIfNoItemsAreLinedUpForBulkShippingUpdate
            ()
        {
            var items = new List<BulkShippingUpdateDataTransferObject>();

            int result = _bulkShippingUpdateService.BulkShippingUpdateFromDTOs(items, false);

            result.Should().Be(0);
        }
    }
}