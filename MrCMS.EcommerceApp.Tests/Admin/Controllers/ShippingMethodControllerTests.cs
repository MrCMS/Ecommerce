using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ShippingMethodControllerTests
    {
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly ITaxRateManager _taxRateManager;
        private readonly ShippingMethodController _shippingMethodController;

        public ShippingMethodControllerTests()
        {
            _shippingMethodManager = A.Fake<IShippingMethodManager>();
            _taxRateManager = A.Fake<ITaxRateManager>();
            _shippingMethodController = new ShippingMethodController(_shippingMethodManager, _taxRateManager);
        }

        [Fact]
        public void ShippingMethodController_Index_ReturnsAViewResult()
        {
            var index = _shippingMethodController.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ShippingMethodController_Index_CallsShippingMethodGetAll()
        {
            _shippingMethodController.Index();

            A.CallTo(() => _shippingMethodManager.GetAll()).MustHaveHappened();
        }

        [Fact]
        public void ShippingMethodController_Index_ReturnsTheResultOfTheShippingMethodManagerCall()
        {
            var shoppingMethods = new List<ShippingMethod>();
            A.CallTo(() => _shippingMethodManager.GetAll()).Returns(shoppingMethods);

            var index = _shippingMethodController.Index();

            index.Model.Should().Be(shoppingMethods);
        }

        [Fact]
        public void ShippingMethodController_AddGet_ReturnsAPartialViewResult()
        {
            var addGet = _shippingMethodController.Add();

            addGet.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingMethodController_Add_CallsAddOnTheShippingMethodManager()
        {
            var shippingMethod = new ShippingMethod();

            var add = _shippingMethodController.Add(shippingMethod);

            A.CallTo(() => _shippingMethodManager.Add(shippingMethod)).MustHaveHappened();
        }

        [Fact]
        public void ShippingMethodController_Add_ReturnsRedirectToRouteResult()
        {
            var option = new ShippingMethod();

            var add = _shippingMethodController.Add(option);

            add.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ShippingMethodController_Add_ReturnsRedirectToIndex()
        {
            var option = new ShippingMethod();

            var add = _shippingMethodController.Add(option);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ShippingMethodController_EditGet_ReturnsAPartialViewResult()
        {
            var shippingMethod = new ShippingMethod();

            var edit = _shippingMethodController.Edit(shippingMethod);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingMethodController_EditGet_ReturnsThePassedOptionAsViewModel()
        {
            var shippingMethod = new ShippingMethod();

            var edit = _shippingMethodController.Edit(shippingMethod);

            edit.Model.Should().Be(shippingMethod);
        }

        [Fact]
        public void ShippingMethodController_EditPost_CallsUpdateOptionOnTheManager()
        {
            var shippingMethod = new ShippingMethod();

            var editPost = _shippingMethodController.Edit_POST(shippingMethod);

            A.CallTo(() => _shippingMethodManager.Update(shippingMethod)).MustHaveHappened();
        }

        [Fact]
        public void ShippingMethodController_EditPost_ReturnsRedirectToRouteResult()
        {
            var shippingMethod = new ShippingMethod();

            var editPost = _shippingMethodController.Edit_POST(shippingMethod);

            editPost.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ShippingMethodController_EditPost_RedirectsToIndex()
        {
            var shippingMethod = new ShippingMethod();

            var editPost = _shippingMethodController.Edit_POST(shippingMethod);

            editPost.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ShippingMethodController_Delete_ReturnsPartialViewResult()
        {
            var shippingMethod = new ShippingMethod();

            var delete = _shippingMethodController.Delete(shippingMethod);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ShippingMethodController_Delete_ReturnsOptionAsModel()
        {
            var shippingMethod = new ShippingMethod();

            var delete = _shippingMethodController.Delete(shippingMethod);

            delete.Model.Should().Be(shippingMethod);
        }

        [Fact]
        public void ShippingMethodController_DeletePOST_CallsDeleteOption()
        {
            var shippingMethod = new ShippingMethod();

            var delete = _shippingMethodController.Delete_POST(shippingMethod);

            A.CallTo(() => _shippingMethodManager.Delete(shippingMethod)).MustHaveHappened();
        }

        [Fact]
        public void ShippingMethodController_DeletePOST_ReturnsRedirectToRouteResult()
        {
            var shippingMethod = new ShippingMethod();

            var delete = _shippingMethodController.Delete_POST(shippingMethod);

            delete.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ShippingMethodController_DeletePOST_RedirectsToIndex()
        {
            var option = new ShippingMethod();

            var delete = _shippingMethodController.Delete_POST(option);

            delete.RouteValues["action"].Should().Be("Index");
        }
    }
}