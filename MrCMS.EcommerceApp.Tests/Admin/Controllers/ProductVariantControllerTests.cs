using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductVariantControllerTests
    {
        private readonly ProductVariantController _productVariantController;
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;

        public ProductVariantControllerTests()
        {
            _productVariantService = A.Fake<IProductVariantService>();
            _taxRateManager = A.Fake<ITaxRateManager>();
            _productVariantController = new ProductVariantController(_productVariantService, _taxRateManager);
        }

        [Fact]
        public void ProductVariantController_Add_ReturnsAPartialViewResult()
        {
            var add = _productVariantController.Add(new Product());

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductVariantController_Add_ReturnsAProductVariantWithPassedProductSet()
        {
            var product = new Product();
            var add = _productVariantController.Add(product);

            add.Model.Should().BeOfType<ProductVariant>();
            add.Model.As<ProductVariant>().Product.Should().Be(product);
        }

        [Fact]
        public void ProductVariantController_Add_ViewDataTaxRatesTaxRateOptions()
        {
            var product = new Product();
            var selectListItems = new List<SelectListItem>();
            A.CallTo(() => _taxRateManager.GetOptions(null)).Returns(selectListItems);
            var add = _productVariantController.Add(product);

            add.ViewData["taxrates"].Should().Be(selectListItems);
        }

        [Fact]
        public void ProductVariantController_AddPost_CallsAddProductVariantService()
        {
            var productVariant = GetNewProductVariant();
            var addPost = _productVariantController.Add_POST(productVariant);

            A.CallTo(() => _productVariantService.Add(productVariant)).MustHaveHappened();
        }

        [Fact]
        public void ProductVariantController_AddPost_ReturnsARedirectToRouteResult()
        {
            var productVariant = GetNewProductVariant();
            var addPost = _productVariantController.Add_POST(productVariant);

            addPost.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductVariantController_AddPost_RedirectsToEditProductId()
        {
            var productVariant = GetNewProductVariant();
            var addPost = _productVariantController.Add_POST(productVariant);

            addPost.RouteValues["action"].Should().Be("Edit");
            addPost.RouteValues["controller"].Should().Be("Webpage");
            addPost.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void ProductVariantController_Edit_ReturnsAPartialViewResult()
        {
            var productVariant = GetNewProductVariant();

            var edit = _productVariantController.Edit(productVariant);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductVariantController_Edit_ShouldReturnThePassedProductVariantAsTheModel()
        {
            var productVariant = GetNewProductVariant();

            var edit = _productVariantController.Edit(productVariant);

            edit.Model.Should().Be(productVariant);
        }

        [Fact]
        public void ProductVariantController_Edit_ViewDataTaxRatesTaxRateOptions()
        {
            var productVariant = GetNewProductVariant();
            var selectListItems = new List<SelectListItem>();
            A.CallTo(() => _taxRateManager.GetOptions(null)).Returns(selectListItems);
            var add = _productVariantController.Edit(productVariant);

            add.ViewData["taxrates"].Should().Be(selectListItems);
        }

        [Fact]
        public void ProductVariantController_EditPost_ShouldCallUpdateMethodOfService()
        {
            var productVariant = GetNewProductVariant();

            var edit = _productVariantController.Edit_POST(productVariant);

            A.CallTo(() => _productVariantService.Update(productVariant)).MustHaveHappened();
        }

        [Fact]
        public void ProductVariantController_EditPost_ShouldReturnRedirectToRouteResult()
        {
            var productVariant = GetNewProductVariant();

            var edit = _productVariantController.Edit_POST(productVariant);

            edit.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductVariantController_EditPost_ShouldRedirectToProductEdit()
        {
            var productVariant = GetNewProductVariant();

            var edit = _productVariantController.Edit_POST(productVariant);

            edit.RouteValues["action"].Should().Be("Edit");
            edit.RouteValues["controller"].Should().Be("Webpage");
            edit.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void ProductVariantController_Delete_ShouldReturnAPartialViewResult()
        {
            var productVariant = GetNewProductVariant();

            var delete = _productVariantController.Delete(productVariant);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductVariantController_Delete_ShouldReturnThePassedModel()
        {
            var productVariant = GetNewProductVariant();

            var delete = _productVariantController.Delete(productVariant);

            delete.Model.Should().Be(productVariant);
        }

        [Fact]
        public void ProductVariantController_DeletePOST_ShouldReturnARedirectToRouteResult()
        {
            var productVariant = GetNewProductVariant();

            var delete = _productVariantController.Delete_POST(productVariant);

            delete.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductVariantController_DeletePOST_ShouldCallDeleteOnService()
        {
            var productVariant = GetNewProductVariant();

            var delete = _productVariantController.Delete_POST(productVariant);

            A.CallTo(() => _productVariantService.Delete(productVariant)).MustHaveHappened();
        }

        [Fact]
        public void ProductVariantController_DeletePOST_ShouldRedirectToEditProduct()
        {
            var productVariant = GetNewProductVariant();

            var delete = _productVariantController.Delete_POST(productVariant);
            
            delete.RouteValues["action"].Should().Be("Edit");
            delete.RouteValues["controller"].Should().Be("Webpage");
            delete.RouteValues["id"].Should().Be(123);
        }

        private static ProductVariant GetNewProductVariant()
        {
            return new ProductVariant { Product = new Product { Id = 123 } };
        }
    }
}