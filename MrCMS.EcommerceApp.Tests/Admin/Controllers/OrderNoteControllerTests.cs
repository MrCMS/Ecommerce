using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class OrderNoteControllerTests : InMemoryDatabaseTest
    {
        private readonly IOrderNoteService _orderNoteService;
        private readonly OrderNoteController _orderNoteController;
        private readonly IOrderService _orderService;

        public OrderNoteControllerTests()
        {
            _orderNoteService = A.Fake<IOrderNoteService>();
            _orderService = A.Fake<IOrderService>();
            _orderNoteController = new OrderNoteController(_orderNoteService, _orderService);
        }

        [Fact]
        public void OrderNoteController_Add_ReturnsPartialViewResult()
        {
            var add = _orderNoteController.Add(1);

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void OrderNoteController_Add_ReturnsAOrderNote()
        {
            var add = _orderNoteController.Add(1);

            add.Model.Should().BeOfType<OrderNote>();
        }

        [Fact]
        public void OrderNoteController_AddPOST_CallsIOrderNoteServiceAddWithPassedOrderNote()
        {
            var item = new OrderNote();
            item.Order = new Order { Id = 1 };

            var add = _orderNoteController.Add_POST(item);

            A.CallTo(() => _orderNoteService.Save(item)).MustHaveHappened();
        }

        [Fact]
        public void OrderNoteController_AddPOST_RedirectsToEditOrderForTheIdOfOrder()
        {
            var orderNote = new OrderNote();
            orderNote.Order = new Order { Id = 1 };

            var add = _orderNoteController.Add_POST(orderNote);

            add.RouteValues["action"].Should().Be("Edit");
            add.RouteValues["controller"].Should().Be("Order");
            add.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void OrderNoteController_Edit_ReturnsAPartialViewResult()
        {
            var item = new OrderNote();

            var result = _orderNoteController.Edit(item);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void OrderNoteController_Edit_ReturnsThePassedObjectAsTheModel()
        {
            var item = new OrderNote();

            var result = _orderNoteController.Edit(item);

            result.As<PartialViewResult>().Model.Should().Be(item);
        }


        [Fact]
        public void OrderNoteController_EditPOST_RedirectsToEditOrderForTheIdOfOrder()
        {
            var orderNote = new OrderNote();
            orderNote.Order = new Order { Id = 1 };

            var edit = _orderNoteController.Edit_POST(orderNote);

            edit.RouteValues["action"].Should().Be("Edit");
            edit.RouteValues["controller"].Should().Be("Order");
            edit.RouteValues["id"].Should().Be(1);
        }


        [Fact]
        public void OrderNoteController_Delete_ReturnsAPartialView()
        {
            var delete = _orderNoteController.Delete(new OrderNote());

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void OrderNoteController_Delete_ShouldReturnPassedOrderNoteAsModel()
        {
            var item = new OrderNote();

            var delete = _orderNoteController.Delete(item);

            delete.Model.Should().Be(item);
        }

        [Fact]
        public void OrderNoteController_DeletePOST_ShouldCallDeleteForThePassedOrderNote()
        {
            var item = new OrderNote();
            item.Order = new Order { Id = 1 };

            var delete = _orderNoteController.Delete_POST(item);

            A.CallTo(() => _orderNoteService.Delete(item)).MustHaveHappened();
        }

        [Fact]
        public void OrderNoteController_DeletePOST_RedirectsToEditOrderForTheIdOfOrder()
        {
            var orderNote = new OrderNote();
            orderNote.Order = new Order { Id = 1 };

            var delete = _orderNoteController.Delete_POST(orderNote);

            delete.RouteValues["action"].Should().Be("Edit");
            delete.RouteValues["controller"].Should().Be("Order");
            delete.RouteValues["id"].Should().Be(1);
        }
    }
}