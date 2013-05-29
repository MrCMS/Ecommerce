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
using MrCMS.Website;
using MrCMS.EcommerceApp.Tests.Services;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductSpecificationAttributeOptionControllerTests
    {
        private IProductOptionManager _productOptionManager;

        [Fact]
        public void ProductSpecificationAttributeOptionController_Index_ReturnsActionResult()
        {
            var controller = GetProductSpecificationAttributeOptionController();

            var index = controller.Index();

            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeOptionController_Add_NotHappensIfNotValidAttributeIdIsSent()
        {
            var controller = GetProductSpecificationAttributeOptionController();
            var attribute = new ProductSpecificationAttribute();
            var option = new ProductSpecificationAttributeOption() { ProductSpecificationAttribute = attribute };

            var add = controller.Add(attribute.Id);

            A.CallTo(() => _productOptionManager.AddSpecificationAttributeOption(option)).MustNotHaveHappened();
        }

        [Fact]
        public void ProductSpecificationAttributeOptionController_EditGet_ReturnsThePassedOptionAsViewModel()
        {
            var controller = GetProductSpecificationAttributeOptionController();
            var option = new ProductSpecificationAttributeOption();

            var edit = controller.Edit(option);

            edit.Model.Should().Be(option);
        }

        [Fact]
        public void ProductSpecificationAttributeOptionController_Delete_ReturnsPartialViewResult()
        {
            var controller = GetProductSpecificationAttributeOptionController();
            var attribute = new ProductSpecificationAttribute();
            var option = new ProductSpecificationAttributeOption() { ProductSpecificationAttribute = attribute };

            var delete = controller.Delete(option);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeOptionController_Delete_ReturnsOptionAsModel()
        {
            var controller = GetProductSpecificationAttributeOptionController();
            var attribute = new ProductSpecificationAttribute();
            var option = new ProductSpecificationAttributeOption() { ProductSpecificationAttribute = attribute };
            
            var delete = controller.Delete(option);

            delete.Model.Should().Be(option);
        }

        [Fact]
        public void ProductSpecificationAttributeOptionController_DeletePOST_CallsDeleteOption()
        {
            var controller = GetProductSpecificationAttributeOptionController();
            var attribute = new ProductSpecificationAttribute();
            var option = new ProductSpecificationAttributeOption() { ProductSpecificationAttribute = attribute };
            
            var delete = controller.Delete_POST(option);

            A.CallTo(() => _productOptionManager.DeleteSpecificationAttributeOption(option)).MustHaveHappened();
        }

        [Fact]
        public void ProductSpecificationAttributeOptionController_DeletePOST_ReturnsRedirectToRouteResult()
        {
            var controller = GetProductSpecificationAttributeOptionController();
            var attribute = new ProductSpecificationAttribute();
            var option = new ProductSpecificationAttributeOption() { ProductSpecificationAttribute = attribute };
            
            var delete = controller.Delete_POST(option);

            delete.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductSpecificationAttributeOptionController_DeletePOST_RedirectsToIndex()
        {
            var controller = GetProductSpecificationAttributeOptionController();
            var attribute = new ProductSpecificationAttribute();
            var option = new ProductSpecificationAttributeOption() { ProductSpecificationAttribute = attribute };
            
            var delete = controller.Delete_POST(option);

            delete.RouteValues["action"].Should().Be("Index");
        }

        ProductSpecificationAttributeOptionController GetProductSpecificationAttributeOptionController()
        {
            _productOptionManager = A.Fake<IProductOptionManager>();
            return new ProductSpecificationAttributeOptionController(_productOptionManager);
        }
    }
}