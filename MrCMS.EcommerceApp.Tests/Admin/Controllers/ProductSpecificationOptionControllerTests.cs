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
    public class ProductSpecificationOptionControllerTests
    {
        private IProductOptionManager _productOptionManager;

        [Fact]
        public void ProductSpecificationOptionController_Index_ReturnsAViewResult()
        {
            var controller = GetProductSpecificationOptionController();

            var index = controller.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductSpecificationOptionController_Index_CallsProductOptionManagerListSpecificationOptions()
        {
            var controller = GetProductSpecificationOptionController();
            
           controller.Index();

            A.CallTo(() => _productOptionManager.ListSpecificationOptions()).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationOptionController_Index_ReturnsTheResultOfTheProductOptionManagerCall()
        {
            var controller = GetProductSpecificationOptionController();
            var productSpecificationOptions = new List<ProductSpecificationOption>();
            A.CallTo(() => _productOptionManager.ListSpecificationOptions()).Returns(productSpecificationOptions);

            var index = controller.Index();

            index.Model.Should().Be(productSpecificationOptions);
        }

        [Fact]
        public void ProductSpecificationOptionController_Add_CallsAddOnTheProductOptionManager()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var add = controller.Add(option);

            A.CallTo(() => _productOptionManager.AddSpecificationOption(option)).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationOptionController_Add_ReturnsRedirectToRouteResult()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var add = controller.Add(option);

            add.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductSpecificationOptionController_Add_ReturnsRedirectToIndex()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var add = controller.Add(option);

            add.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ProductSpecificationOptionController_EditGet_ReturnsAPartialViewResult()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var edit = controller.Edit(option);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductSpecificationOptionController_EditGet_ReturnsThePassedOptionAsViewModel()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var edit = controller.Edit(option);

            edit.Model.Should().Be(option);
        }

        [Fact]
        public void ProductSpecificationOptionController_EditPost_CallsUpdateOptionOnTheManager()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var editPost = controller.Edit_POST(option);

            A.CallTo(() => _productOptionManager.UpdateSpecificationOption(option)).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationOptionController_EditPost_ReturnsRedirectToRouteResult()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var editPost = controller.Edit_POST(option);

            editPost.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductSpecificationOptionController_EditPost_RedirectsToIndex()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var editPost = controller.Edit_POST(option);

            editPost.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void ProductSpecificationOptionController_Delete_ReturnsPartialViewResult()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();

            var delete = controller.Delete(option);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductSpecificationOptionController_Delete_ReturnsOptionAsModel()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();
            
            var delete = controller.Delete(option);

            delete.Model.Should().Be(option);
        }

        [Fact]
        public void ProductSpecificationOptionController_DeletePOST_CallsDeleteOption()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();
            
            var delete = controller.Delete_POST(option);

            A.CallTo(() => _productOptionManager.DeleteSpecificationOption(option)).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationOptionController_DeletePOST_ReturnsRedirectToRouteResult()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();
            
            var delete = controller.Delete_POST(option);

            delete.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductSpecificationOptionController_DeletePOST_RedirectsToIndex()
        {
            var controller = GetProductSpecificationOptionController();
            var option = new ProductSpecificationOption();
            
            var delete = controller.Delete_POST(option);

            delete.RouteValues["action"].Should().Be("Index");
        }

        ProductSpecificationOptionController GetProductSpecificationOptionController()
        {
            _productOptionManager = A.Fake<IProductOptionManager>();
            return new ProductSpecificationOptionController(_productOptionManager);
        }
    }
}