using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ShippingMethodControllerTests
    {
        private IShippingMethodManager _shippingMethodManager;

        [Fact]
        public void ShippingMethodController_Index_ReturnsAViewResult()
        {
            var controller = GetShippingMethodController();

            var index = controller.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ShippingMethodController_Index_CallsShippingMethodGetAll()
        {
            var controller = GetShippingMethodController();

            controller.Index();

            A.CallTo(() => _shippingMethodManager.GetAll()).MustHaveHappened();
        }

        [Fact]
        public void ShippingMethodController_Index_ReturnsTheResultOfTheShippingMethodManagerCall()
        {
            var controller = GetShippingMethodController();
            var shoppingMethods = new List<ShippingMethod>();
            A.CallTo(() => _shippingMethodManager.GetAll()).Returns(shoppingMethods);

            var index = controller.Index();

            index.Model.Should().Be(shoppingMethods);
        }

        [Fact]
        public void ShippingMethodController_AddGet_ReturnsAPartialViewResult()
        {
            var controller = GetShippingMethodController();

            var addGet = controller.Add();

            addGet.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingMethodController_Add_CallsAddOnTheShippingMethodManager()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var add = controller.Add(shippingMethod);

            A.CallTo(() => _shippingMethodManager.Add(shippingMethod)).MustHaveHappened();
        }

        [Fact]
        public void ShippingMethodController_Add_ReturnsRedirectToRouteResult()
        {
            var controller = GetShippingMethodController();
            var option = new ShippingMethod();

            var add = controller.Add(option);

            add.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ShippingMethodController_Add_ReturnsRedirectToIndex()
        {
            var controller = GetShippingMethodController();
            var option = new ShippingMethod();

            var add = controller.Add(option);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ShippingMethodController_EditGet_ReturnsAPartialViewResult()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var edit = controller.Edit(shippingMethod);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingMethodController_EditGet_ReturnsThePassedOptionAsViewModel()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var edit = controller.Edit(shippingMethod);

            edit.Model.Should().Be(shippingMethod);
        }

        [Fact]
        public void ShippingMethodController_EditPost_CallsUpdateOptionOnTheManager()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var editPost = controller.Edit_POST(shippingMethod);

            A.CallTo(() => _shippingMethodManager.Update(shippingMethod)).MustHaveHappened();
        }

        [Fact]
        public void ShippingMethodController_EditPost_ReturnsRedirectToRouteResult()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var editPost = controller.Edit_POST(shippingMethod);

            editPost.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ShippingMethodController_EditPost_RedirectsToIndex()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var editPost = controller.Edit_POST(shippingMethod);

            editPost.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ShippingMethodController_Delete_ReturnsPartialViewResult()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var delete = controller.Delete(shippingMethod);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingMethodController_Delete_ReturnsOptionAsModel()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var delete = controller.Delete(shippingMethod);

            delete.Model.Should().Be(shippingMethod);
        }

        [Fact]
        public void ShippingMethodController_DeletePOST_CallsDeleteOption()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var delete = controller.Delete_POST(shippingMethod);

            A.CallTo(() => _shippingMethodManager.Delete(shippingMethod)).MustHaveHappened();
        }

        [Fact]
        public void ShippingMethodController_DeletePOST_ReturnsRedirectToRouteResult()
        {
            var controller = GetShippingMethodController();
            var shippingMethod = new ShippingMethod();

            var delete = controller.Delete_POST(shippingMethod);

            delete.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ShippingMethodController_DeletePOST_RedirectsToIndex()
        {
            var controller = GetShippingMethodController();
            var option = new ShippingMethod();
            
            var delete = controller.Delete_POST(option);

            delete.RouteValues["action"].Should().Be("Index");
        }

        ShippingMethodController GetShippingMethodController()
        {
            _shippingMethodManager = A.Fake<IShippingMethodManager>();
            return new ShippingMethodController(_shippingMethodManager);
        }
    }
}