using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using NHibernate;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
namespace MrCMS.EcommerceApp.Tests.Services
{
    public class OrderShippingServiceTests : InMemoryDatabaseTest
    {
        private readonly ISession _session;
        private readonly IOrderShippingService _orderShippingService;
        private readonly IBulkShippingUpdateValidationService _bulkShippingUpdateValidationService;
        private readonly IBulkShippingUpdateService _bulkShippingUpdateService;

        public OrderShippingServiceTests()
        {
            _bulkShippingUpdateValidationService =  A.Fake<IBulkShippingUpdateValidationService>(); 
            _bulkShippingUpdateService =  A.Fake<IBulkShippingUpdateService>();
            _session = A.Fake<ISession>();
            _orderShippingService = new OrderShippingService(_session,_bulkShippingUpdateValidationService, _bulkShippingUpdateService);
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