using FakeItEasy;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Services.Orders;

namespace MrCMS.AmazonApp.Tests.Services.Orders
{
    public class AmazonOrderServiceTests : InMemoryDatabaseTest
    {
        private IAmazonLogService _amazonLogService;
        private AmazonOrderService _amazonOrderService;
        private AmazonOrderEventService _amazonOrderEventService;

        public AmazonOrderServiceTests()
        {
            _amazonLogService = A.Fake<IAmazonLogService>();
            _amazonOrderEventService = A.Fake<AmazonOrderEventService>();
            _amazonOrderService = new AmazonOrderService(Session, _amazonLogService, _amazonOrderEventService);
        }

    }
}