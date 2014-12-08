using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Analytics
{
    public class AmazonAnalyticsServiceTests : InMemoryDatabaseTest
    {
        private readonly IAmazonApiLogService _amazonApiUsageService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;

        public AmazonAnalyticsServiceTests()
        {
            _amazonAppSettings = A.Fake<AmazonAppSettings>();
            _amazonSellerSettings = A.Fake<AmazonSellerSettings>();
            _amazonApiUsageService = A.Fake<IAmazonApiLogService>();
            _amazonAnalyticsService = new AmazonAnalyticsService(_amazonApiUsageService, Session, _amazonAppSettings,
                                                                 _amazonSellerSettings);
        }

        [Fact]
        public void AmazonAnalyticsService_GetRevenue_ShouldReturnEntriesGroupedByWeekDay()
        {
            var items = Enumerable.Range(0, 9).Select(i => new AmazonOrder()
                {
                    OrderTotalAmount = 1,
                    NumberOfItemsShipped = 1,
                    NumberOfItemsUnshipped = 0,
                    PurchaseDate = CurrentRequestData.Now.Date.AddDays(i),
                    CreatedOn = CurrentRequestData.Now.Date.AddDays(i),
                    Site = CurrentRequestData.CurrentSite
                }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonAnalyticsService.GetRevenue(CurrentRequestData.Now.Date.AddDays(-20),
                                                             CurrentRequestData.Now.Date.AddDays(20));

            results.Data.Count.Should().Be(7);
        }

        [Fact]
        public void AmazonAnalyticsService_GetProductsSold_ShouldReturnEntriesGroupedByWeekDay()
        {
            var items = Enumerable.Range(0, 9).Select(i => new AmazonOrder()
            {
                OrderTotalAmount = 1,
                NumberOfItemsShipped = 1,
                NumberOfItemsUnshipped = 0,
                PurchaseDate = CurrentRequestData.Now.Date.AddDays(i),
                CreatedOn = CurrentRequestData.Now.Date.AddDays(i),
                Site = CurrentRequestData.CurrentSite
            }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonAnalyticsService.GetProductsSold(CurrentRequestData.Now.Date.AddDays(-20),
                                                             CurrentRequestData.Now.Date.AddDays(20));

            results.Data.Count.Should().Be(7);
        }

        [Fact]
        public void AmazonAnalyticsService_GetNumberOfOrders_ShouldReturnPersistedEntriesCount()
        {
            var items = Enumerable.Range(0, 10).Select(i => new AmazonOrder()
            {
                OrderTotalAmount = 1,
                NumberOfItemsShipped = 1,
                NumberOfItemsUnshipped = 0,
                PurchaseDate = CurrentRequestData.Now.Date.AddDays(1),
                CreatedOn = CurrentRequestData.Now.Date.AddDays(1),
                Site = CurrentRequestData.CurrentSite
            }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonAnalyticsService.GetNumberOfOrders(CurrentRequestData.Now.Date.AddDays(-20),
                                                             CurrentRequestData.Now.Date.AddDays(20));

            results.Should().Be(10);
        }

        [Fact]
        public void AmazonAnalyticsService_GetAverageOrderAmount_ShouldReturnAvgAmount()
        {
            var items = Enumerable.Range(0, 10).Select(i => new AmazonOrder()
            {
                OrderTotalAmount = Decimal.Parse("0.5"),
                NumberOfItemsShipped = 1,
                NumberOfItemsUnshipped = 0,
                PurchaseDate = CurrentRequestData.Now.Date.AddDays(i),
                CreatedOn = CurrentRequestData.Now.Date.AddDays(i),
                Site = CurrentRequestData.CurrentSite
            }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonAnalyticsService.GetAverageOrderAmount(CurrentRequestData.Now.Date.AddDays(-20),
                                                             CurrentRequestData.Now.Date.AddDays(20));

            results.Should().Be(Double.Parse("0.5"));
        }

        [Fact]
        public void AmazonAnalyticsService_GetNumberUnshippedOrders_ShouldReturnPersistedEntriesUnshippedCount()
        {
            var items = Enumerable.Range(0, 10).Select(i => new AmazonOrder()
            {
                OrderTotalAmount = 1,
                NumberOfItemsShipped = 1,
                NumberOfItemsUnshipped = 0,
                Status = i>=5?AmazonOrderStatus.Shipped : AmazonOrderStatus.Unshipped,
                PurchaseDate = CurrentRequestData.Now.Date.AddDays(1),
                CreatedOn = CurrentRequestData.Now.Date.AddDays(1),
                Site = CurrentRequestData.CurrentSite
            }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonAnalyticsService.GetNumberUnshippedOrders(CurrentRequestData.Now.Date.AddDays(-20),
                                                             CurrentRequestData.Now.Date.AddDays(20));

            results.Should().Be(5);
        }

        [Fact]
        public void AmazonAnalyticsService_GetNumberOfOrderedProducts_ShouldReturnPersistedEntriesOrderedProductsCount()
        {
            var items = Enumerable.Range(0, 10).Select(i => new AmazonOrder()
            {
                OrderTotalAmount = 1,
                NumberOfItemsShipped = 1,
                NumberOfItemsUnshipped = 0,
                Status = i >= 5 ? AmazonOrderStatus.Shipped : AmazonOrderStatus.Unshipped,
                PurchaseDate = CurrentRequestData.Now.Date.AddDays(1),
                CreatedOn = CurrentRequestData.Now.Date.AddDays(1),
                Site = CurrentRequestData.CurrentSite,
            }).ToList();
            foreach (var amazonOrder in items)
            {
                amazonOrder.Items = new List<AmazonOrderItem>()
                    {
                        new AmazonOrderItem()
                            {
                                AmazonOrder = amazonOrder,
                                QuantityOrdered = 1,
                                QuantityShipped = 1
                            }
                    };
            }
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonAnalyticsService.GetNumberOfOrderedProducts(CurrentRequestData.Now.Date.AddDays(-20),
                                                             CurrentRequestData.Now.Date.AddDays(20));

            results.Should().Be(10);
        }

        [Fact]
        public void AmazonAnalyticsService_GetNumberOfShippedProducts_ShouldReturnPersistedEntriesShippedProductsCount()
        {
            var items = Enumerable.Range(0, 10).Select(i => new AmazonOrder()
            {
                OrderTotalAmount = 1,
                NumberOfItemsShipped = 1,
                NumberOfItemsUnshipped = 0,
                Status = i >= 5 ? AmazonOrderStatus.Shipped : AmazonOrderStatus.Unshipped,
                PurchaseDate = CurrentRequestData.Now.Date.AddDays(1),
                CreatedOn = CurrentRequestData.Now.Date.AddDays(1),
                Site = CurrentRequestData.CurrentSite,
            }).ToList();
            foreach (var amazonOrder in items)
            {
                amazonOrder.Items = new List<AmazonOrderItem>()
                    {
                        new AmazonOrderItem()
                            {
                                AmazonOrder = amazonOrder,
                                QuantityOrdered = 1,
                                QuantityShipped = 1
                            }
                    };
            }
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonAnalyticsService.GetNumberOfShippedProducts(CurrentRequestData.Now.Date.AddDays(-20),
                                                             CurrentRequestData.Now.Date.AddDays(20));

            results.Should().Be(10);
        }

        [Fact]
        public void AmazonAnalyticsService_GetNumberOfActiveListings_ShouldReturnPersistedEntriesActive()
        {
            var items = Enumerable.Range(0, 10).Select(i => new AmazonListing()
                {
                    Status = AmazonListingStatus.Active
                }).ToList();
            Session.Transact(session => items.ForEach(item => session.Save(item)));

            var results = _amazonAnalyticsService.GetNumberOfActiveListings();

            results.Should().Be(10);
        }
    }
}