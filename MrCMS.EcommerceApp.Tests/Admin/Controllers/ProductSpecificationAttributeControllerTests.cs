using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductSpecificationAttributeControllerTests
    {
        private IProductOptionManager _productOptionManager;

        [Fact]
        public void ProductSpecificationAttributeController_Index_ReturnsAViewResult()
        {
            var controller = GetProductSpecificationAttributeController();

            var index = controller.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeController_Index_CallsProductOptionManagerListSpecificationAttributes()
        {
            var controller = GetProductSpecificationAttributeController();
            
           controller.Index();

            A.CallTo(() => _productOptionManager.ListSpecificationAttributes()).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationAttributeController_Index_ReturnsTheResultOfTheProductOptionManagerCall()
        {
            var controller = GetProductSpecificationAttributeController();
            var productSpecificationAttributes = new List<ProductSpecificationAttribute>();
            A.CallTo(() => _productOptionManager.ListSpecificationAttributes()).Returns(productSpecificationAttributes);

            var index = controller.Index();

            index.Model.Should().Be(productSpecificationAttributes);
        }

        [Fact]
        public void ProductSpecificationAttributeController_AddGet_ReturnsAPartialViewResult()
        {
            var controller = GetProductSpecificationAttributeController();

            var addGet = controller.Add();

            addGet.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeController_Add_CallsAddOnTheProductOptionManager()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();

            var add = controller.Add(option);

            A.CallTo(() => _productOptionManager.AddSpecificationAttribute(option)).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationAttributeController_Add_ReturnsRedirectToRouteResult()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();

            var add = controller.Add(option);

            add.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeController_EditGet_ReturnsAPartialViewResult()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();

            var edit = controller.Edit(option);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeController_EditGet_ReturnsThePassedOptionAsViewModel()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();

            var edit = controller.Edit(option);

            edit.Model.Should().Be(option);
        }

        [Fact]
        public void ProductSpecificationAttributeController_EditPost_CallsUpdateOptionOnTheManager()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();

            var editPost = controller.Edit_POST(option);

            A.CallTo(() => _productOptionManager.UpdateSpecificationAttribute(option)).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationAttributeController_EditPost_ReturnsRedirectToRouteResult()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();

            var editPost = controller.Edit_POST(option);

            editPost.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeController_Delete_ReturnsPartialViewResult()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();

            var delete = controller.Delete(option);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeController_Delete_ReturnsOptionAsModel()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();
            
            var delete = controller.Delete(option);

            delete.Model.Should().Be(option);
        }

        [Fact]
        public void ProductSpecificationAttributeController_DeletePOST_CallsDeleteOption()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();
            
            var delete = controller.Delete_POST(option);

            A.CallTo(() => _productOptionManager.DeleteSpecificationAttribute(option)).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationAttributeController_DeletePOST_ReturnsRedirectToRouteResult()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();
            
            var delete = controller.Delete_POST(option);

            delete.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeController_DeletePOST_RedirectsToIndex()
        {
            var controller = GetProductSpecificationAttributeController();
            var option = new ProductSpecificationAttribute();
            
            var delete = controller.Delete_POST(option);

            delete.RouteValues["action"].Should().Be("Index");
        }

        ProductSpecificationAttributeController GetProductSpecificationAttributeController()
        {
            _productOptionManager = A.Fake<IProductOptionManager>();
            return new ProductSpecificationAttributeController(_productOptionManager);
        }
    }
}