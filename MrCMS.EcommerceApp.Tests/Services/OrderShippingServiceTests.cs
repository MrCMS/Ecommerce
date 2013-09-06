using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
namespace MrCMS.EcommerceApp.Tests.Services
{
    public class OrderShippingServiceTests : InMemoryDatabaseTest
    {
        private readonly IOrderShippingService _orderShippingService;
        private readonly IBulkShippingUpdateValidationService _bulkShippingUpdateValidationService;
        private readonly IBulkShippingUpdateService _bulkShippingUpdateService;
        private readonly IUserService _userService;

        public OrderShippingServiceTests()
        {
            _bulkShippingUpdateValidationService = A.Fake<IBulkShippingUpdateValidationService>();
            _bulkShippingUpdateService = A.Fake<IBulkShippingUpdateService>();
            _userService = A.Fake<IUserService>();
            _orderShippingService = new OrderShippingService(Session, _bulkShippingUpdateValidationService, _bulkShippingUpdateService, _userService);
        }

        [Fact]
        public void OrderShippingService_BulkShippingUpdate_ShouldNotBeNull()
        {
            var result = _orderShippingService.BulkShippingUpdate(GetDefaultStream());

            result.Should().NotBeNull();
        }

        [Fact]
        public void OrderShippingService_BulkShippingUpdate_ShouldReturnDictionary()
        {
            var result = _orderShippingService.BulkShippingUpdate(GetDefaultStream());

            result.Should().HaveCount(1);
        }

        private static Stream GetDefaultStream()
        {
            return new MemoryStream(0);
        }
    }
}