using FakeItEasy;
using MrCMS.EcommerceApp.Tests;
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