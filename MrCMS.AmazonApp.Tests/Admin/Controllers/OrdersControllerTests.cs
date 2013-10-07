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
using MrCMS.Web.Apps.Ecommerce.Settings;
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
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly OrdersController _ordersController;
        private IAmazonOrderSyncService _amazonOrderSyncService;

        public OrdersControllerTests()
        {
            _syncAmazonOrderService = A.Fake<IAmazonOrderSyncManager>();
            _amazonOrderService = A.Fake<IAmazonOrderService>();
            _amazonAppSettings = A.Fake<AmazonAppSettings>();
            _optionService = A.Fake<IOptionService>();
            _amazonOrderSearchService = A.Fake<IAmazonOrderSearchService>();
            _ecommerceSettings = new EcommerceSettings();
            _amazonOrderSyncService = A.Fake<IAmazonOrderSyncService>();
            _ordersController = new OrdersController(_amazonOrderService,_amazonAppSettings,_amazonOrderSearchService,_ecommerceSettings,_amazonOrderSyncService);
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
            var dateTime = DateTime.Now;
            var model = new AmazonOrderSearchModel()
                {
                    DateFrom = dateTime,
                    DateTo = dateTime,
                    ShippingStatus = ShippingStatus.Pending
                };

            var result = _ordersController.Index(model);

            A.CallTo(() => _amazonOrderSearchService.Search(model, model.Page, 10)).MustHaveHappened();
        }

        [Fact]
        public void OrdersController_Orders_ShouldCallSearch()
        {
            var dateTime = DateTime.Now;
            var model = new AmazonOrderSearchModel()
            {
                DateFrom = dateTime,
                DateTo = dateTime,
                ShippingStatus = ShippingStatus.Pending
            };

            var result = _ordersController.Orders(model);

            A.CallTo(() => _amazonOrderSearchService.Search(model, model.Page, 10)).MustHaveHappened();
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

    

    }
}