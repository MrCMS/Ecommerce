using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.EcommerceApp.Tests.Services.BulkShippingUpdate
{
    public class BulkShippingUpdateServiceTests : InMemoryDatabaseTest
    {
        private readonly BulkShippingUpdateService _bulkShippingUpdateService;
        private readonly IOrderService _orderService;
        private readonly IShippingMethodManager _shippingMethodManager;

        public BulkShippingUpdateServiceTests()
        {
            _orderService = A.Fake<IOrderService>();
            _shippingMethodManager = A.Fake<IShippingMethodManager>();
             _bulkShippingUpdateService = new BulkShippingUpdateService(_orderService,_shippingMethodManager);
        }

        [Fact]
        public void BulkShippingUpdateService_BulkShippingUpdateFromDTOs_ShouldReturnZeroIfNoItemsAreLinedUpForBulkShippingUpdate()
        {
            var items = new List<BulkShippingUpdateDataTransferObject>();

            var result = _bulkShippingUpdateService.BulkShippingUpdateFromDTOs(items);

            result.Should().Be(0);
        }

        [Fact]
        public void BulkShippingUpdateService_BulkShippingUpdate_ShouldReturnOrder()
        {
            var noOfUpdatedItems = 0;
            var item = new BulkShippingUpdateDataTransferObject()
                {
                    OrderId = 1,
                    ShippingMethod = "Test"
                };

            A.CallTo(() => _orderService.Get(item.OrderId)).Returns(new Order());

            var result = _bulkShippingUpdateService.BulkShippingUpdate(item, ref noOfUpdatedItems);

            result.Should().BeOfType<Order>();
        }

    }
}