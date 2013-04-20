using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Payments;
using MrCMS.EcommerceApp.Tests.Services;
using MrCMS.Entities.People;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class OrderLineControllerTests : InMemoryDatabaseTest
    {
        private readonly IOrderLineService _orderLineService;
        private readonly OrderLineController _orderLineController;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrderLineControllerTests()
        {
            A.CallTo(() => CurrentRequestData.CurrentContext.Session).Returns(new FakeHttpSessionState());
            _orderLineService = A.Fake<IOrderLineService>();
            _orderService = A.Fake<IOrderService>();
            _productService = A.Fake<IProductService>();
            _orderLineController = new OrderLineController(_orderLineService, _orderService,_productService);
        }

        [Fact]
        public void OrderLineController_Add_ReturnsPartialViewResult()
        {
            var add = _orderLineController.Add(1);

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void OrderLineController_Add_ReturnsAOrderLine()
        {
            var add = _orderLineController.Add(1);

            add.Model.Should().BeOfType<OrderLine>();
        }

        [Fact]
        public void OrderLineController_AddPOST_RedirectsToEditOrderForTheIdOfOrder()
        {
            var orderLine = new OrderLine();
            orderLine.Order = new Order { Id = 1 };

            var add = _orderLineController.Add_POST(orderLine);

            add.RouteValues["action"].Should().Be("Edit");
            add.RouteValues["controller"].Should().Be("Order");
            add.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void OrderLineController_Edit_ReturnsAPartialViewResult()
        {
            var item = new OrderLine();
            item.ProductVariant = new Product() { Id = 1 };

            var result = _orderLineController.Edit(item);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void OrderLineController_Edit_ReturnsThePassedObjectAsTheModel()
        {
            var item = new OrderLine();
            item.ProductVariant = new Product() { Id = 1 };

            var result = _orderLineController.Edit(item);

            result.As<PartialViewResult>().Model.Should().Be(item);
        }


        [Fact]
        public void OrderLineController_EditPOST_RedirectsToEditOrderForTheIdOfOrder()
        {
            var orderLine = new OrderLine();
            orderLine.Order = new Order { Id = 1 };

            var edit = _orderLineController.Edit_POST(orderLine,1);

            edit.RouteValues["action"].Should().Be("Edit");
            edit.RouteValues["controller"].Should().Be("Order");
            edit.RouteValues["id"].Should().Be(1);
        }


        [Fact]
        public void OrderLineController_Delete_ReturnsAPartialView()
        {
            var delete = _orderLineController.Delete(new OrderLine());

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void OrderLineController_Delete_ShouldReturnPassedOrderLineAsModel()
        {
            var item = new OrderLine();

            var delete = _orderLineController.Delete(item);

            delete.Model.Should().Be(item);
        }

        [Fact]
        public void OrderLineController_DeletePOST_ShouldCallDeleteForThePassedOrderLine()
        {
            var item = new OrderLine();
            item.Order = new Order { Id = 1 };

            var delete = _orderLineController.Delete_POST(item);

            A.CallTo(() => _orderLineService.Delete(item)).MustHaveHappened();
        }

        [Fact]
        public void OrderLineController_DeletePOST_RedirectsToEditOrderForTheIdOfOrder()
        {
            var orderLine = new OrderLine();
            orderLine.Order = new Order { Id = 1 };

            var delete = _orderLineController.Delete_POST(orderLine);

            delete.RouteValues["action"].Should().Be("Edit");
            delete.RouteValues["controller"].Should().Be("Order");
            delete.RouteValues["id"].Should().Be(1);
        }
    }
}