using System;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Admin.Controllers
{
    public class OrdersControllerTests : InMemoryDatabaseTest
    {
        private readonly IAmazonOrderSyncManager _syncAmazonOrderService;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly IOptionService _optionService;
        private readonly IAmazonOrderSearchService _amazonOrderSearchService;
        private readonly OrdersController _ordersController;

        public OrdersControllerTests()
        {
            _syncAmazonOrderService = A.Fake<IAmazonOrderSyncManager>();
            _amazonOrderService = A.Fake<IAmazonOrderService>();
            _amazonAppSettings = A.Fake<AmazonAppSettings>();
            _optionService = A.Fake<IOptionService>();
            _amazonOrderSearchService = A.Fake<IAmazonOrderSearchService>();
            _ordersController = new OrdersController(_syncAmazonOrderService,_amazonOrderService,_amazonAppSettings,_optionService,_amazonOrderSearchService);
        }

        [Fact]
        public void OrdersController_Index_ReturnsViewResult()
        {
            var result = _ordersController.Index(new AmazonOrderSearchModel());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrdersController_Index_ShouldCallSearch()
        {
            var model = new AmazonOrderSearchModel()
                {
                    DateFrom = DateTime.Now,
                    DateTo = DateTime.Now,
                    ShippingStatus = ShippingStatus.Pending
                };

            var result = _ordersController.Index(model);

            A.CallTo(() => _amazonOrderSearchService.Search(model.Email, model.Name, model.AmazonOrderId,
                    model.DateFrom.HasValue ? model.DateFrom.Value : DateTime.Now, model.DateTo.HasValue ? model.DateTo.Value : DateTime.Now,
                    model.ShippingStatus, model.Page,10)).MustHaveHappened();
        }

        [Fact]
        public void OrdersController_Orders_ShouldCallSearch()
        {
            var model = new AmazonOrderSearchModel()
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now,
                ShippingStatus = ShippingStatus.Pending
            };

            var result = _ordersController.Orders(model);

            A.CallTo(() => _amazonOrderSearchService.Search(model.Email, model.Name, model.AmazonOrderId,
                    model.DateFrom.HasValue ? model.DateFrom.Value : DateTime.Now, model.DateTo.HasValue ? model.DateTo.Value : DateTime.Now,
                    model.ShippingStatus, model.Page, 10)).MustHaveHappened();
        }

        [Fact]
        public void OrdersController_Details_ReturnsViewResult()
        {
            var result = _ordersController.Details(new AmazonOrder(){Id=1});

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrdersController_Details_ShouldRedirectToIndexIfNoListingProvided()
        {
            var result = _ordersController.Details(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void OrdersController_SyncMany_ReturnsViewResult()
        {
            var result = _ordersController.SyncMany();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrdersController_SyncOne_ReturnsViewResult()
        {
            var result = _ordersController.SyncOne(new AmazonOrder(){Id=1});

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrdersController_SyncOne_ShouldRedirectToIndexIfNoOrderProvided()
        {
            var result = _ordersController.SyncOne(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void OrdersController_Sync_ShouldReturnJsonResult()
        {
            var result = _ordersController.Sync(new AmazonSyncModel());

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void OrdersController_Sync_ShouldReturnFalseIfSyncModelIsNull()
        {
            var result = _ordersController.Sync(null);

            result.As<JsonResult>().Data.Should().Be(false);
        }

        [Fact]
        public void OrdersController_Sync_ShouldReturnTrueIfSyncModelIsNotNull()
        {
            var result = _ordersController.Sync(new AmazonSyncModel());

            result.As<JsonResult>().Data.Should().Be(true);
        }

        [Fact]
        public void OrdersController_ShipOne_ReturnsViewResult()
        {
            var result = _ordersController.ShipOne_GET(new AmazonOrder(){Id=1,AmazonOrderId = "O1"});

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void OrdersController_ShipOne_ShouldRedirectToIndexIfNoOrderProvided()
        {
            var result = _ordersController.ShipOne_GET(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void OrdersController_ShipOne_ShouldReturnJsonResult()
        {
            var result = _ordersController.ShipOne(new AmazonSyncModel());

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void OrdersController_ShipOne_ShouldReturnFalseIfSyncModelIsNull()
        {
            var result = _ordersController.ShipOne(null);

            result.As<JsonResult>().Data.Should().Be(false);
        }

        [Fact]
        public void OrdersController_ShipOne_ShouldReturnTrueIfSyncModelIsNotNull()
        {
            var result = _ordersController.ShipOne(new AmazonSyncModel(){Id=1,TaskId = Guid.NewGuid()});

            result.As<JsonResult>().Data.Should().Be(true);
        }

    }
}