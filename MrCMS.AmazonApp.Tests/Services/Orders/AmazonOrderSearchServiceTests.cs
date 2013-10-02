using FakeItEasy;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Indexing.Querying;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Indexing;
using MrCMS.Web.Apps.Amazon.Services.Orders;

namespace MrCMS.AmazonApp.Tests.Services.Orders
{
    public class AmazonOrderSearchServiceTests : InMemoryDatabaseTest
    {
        private ISearcher<AmazonOrder, AmazonOrderSearchIndex> _orderSearcher;
        private AmazonOrderSearchService _amazonOrderSearchService;

        public AmazonOrderSearchServiceTests()
        {
            _orderSearcher = A.Fake<ISearcher<AmazonOrder, AmazonOrderSearchIndex>>();
            _amazonOrderSearchService = new AmazonOrderSearchService(_orderSearcher);
        }

    }
}