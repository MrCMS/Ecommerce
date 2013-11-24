using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Orders
{
    public class AmazonOrderServiceTests : InMemoryDatabaseTest
    {
        private IAmazonLogService _amazonLogService;
        private AmazonOrderService _amazonOrderService;
        private IAmazonOrderEventService _amazonOrderEventService;

        public AmazonOrderServiceTests()
        {
            _amazonLogService = A.Fake<IAmazonLogService>();
            _amazonOrderEventService = A.Fake<IAmazonOrderEventService>();
            _amazonOrderService = new AmazonOrderService(Session, _amazonLogService, _amazonOrderEventService);
        }

        [Fact]
        public void AmazonOrderService_Get_ShouldReturnPersistedEntryFromSession()
        {
            var item = new AmazonOrder();
            Session.Transact(session => session.Save(item));

            var results = _amazonOrderService.Get(1);

            results.As<AmazonOrder>().Id.Should().Be(1);
        }

        [Fact]
        public void AmazonOrderService_GetByAmazonOrderId_ShouldReturnPersistedEntryFromSession()
        {
            var item = new AmazonOrder() { AmazonOrderId = "T1" };
            Session.Transact(session => session.Save(item));

            var results = _amazonOrderService.GetByAmazonOrderId("T1");

            results.As<AmazonOrder>().Id.Should().Be(1);
        }

        [Fact]
        public void AmazonOrderService_Search_ShouldReturnPersistedEntries()
        {
            var items = Enumerable.Range(0, 10).Select(i => new AmazonOrder()
            {
                AmazonOrderId = i.ToString()
            }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonOrderService.Search();

            results.Should().HaveCount(10);
        }

        [Fact]
        public void AmazonOrderService_Update_ShouldUpdateInSession()
        {
            var item = new AmazonOrder();
            Session.Transact(session => session.Save(item));
            item.BuyerName = "T1";

            _amazonOrderService.Update(item);
            Session.Evict(item);

            Session.Get<AmazonOrder>(1).BuyerName.Should().Be("T1");
        }

        [Fact]
        public void AmazonOrderService_Update_ShouldCallAddLog()
        {
            var item = new AmazonOrder(){AmazonOrderId = "1"};
            Session.Transact(session => session.Save(item));

            _amazonOrderService.Update(item);

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Update,
                                 null, null, null, null, item, null, null,
                                 "Amazon Order #" + item.AmazonOrderId,string.Empty)
                                  ).MustHaveHappened();
        }

        [Fact]
        public void AmazonOrderService_Delete_ShouldRemoveItemFromTheSession()
        {
            var item = new AmazonOrder();
            Session.Transact(session => session.Save(item));

            _amazonOrderService.Delete(item);

            Session.QueryOver<AmazonOrder>().RowCount().Should().Be(0);
        }

        [Fact]
        public void AmazonOrderService_Delete_ShouldCallAddLog()
        {
            var item = new AmazonOrder(){AmazonOrderId = "1"};
            Session.Transact(session => session.Save(item));

            _amazonOrderService.Delete(item);

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Delete,
                null, null, null, null, item, null, null, "Amazon Order #" + 
                item.AmazonOrderId,string.Empty)).MustHaveHappened();
        }
    }
}