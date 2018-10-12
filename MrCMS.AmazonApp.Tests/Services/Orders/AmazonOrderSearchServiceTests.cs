using FakeItEasy;
using MrCMS.Web.Apps.Amazon.Services.Orders;

namespace MrCMS.AmazonApp.Tests.Services.Orders
{
    public class AmazonOrderSearchServiceTests : InMemoryDatabaseTest
    {
        private IAmazonOrderSearchService _amazonOrderSearchService;

        public AmazonOrderSearchServiceTests()
        {
            _amazonOrderSearchService = A.Fake<IAmazonOrderSearchService>();
        }
    }
}