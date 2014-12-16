using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ProductVariantControllerTests
    {
        private readonly IProductVariantAdminService _productVariantAdminService;
        private readonly ProductVariantController _productVariantController;
        private readonly IProductReviewUIService _productReviewUIService;

        public ProductVariantControllerTests()
        {
            _productVariantAdminService = A.Fake<IProductVariantAdminService>();
            _productReviewUIService = A.Fake<IProductReviewUIService>();
            _productVariantController = new ProductVariantController(_productVariantAdminService, _productReviewUIService);
        }

        [Fact]
        public void ProductVariantController_Add_ReturnsAPartialViewResult()
        {
            PartialViewResult add = _productVariantController.Add(new Product());

            add.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductVariantController_Add_ReturnsAProductVariantWithPassedProductSet()
        {
            var product = new Product();
            PartialViewResult add = _productVariantController.Add(product);

            add.Model.Should().BeOfType<ProductVariant>();
            add.Model.As<ProductVariant>().Product.Should().Be(product);
        }

        [Fact]
        public void ProductVariantController_Add_CallsIntoAddViewData()
        {
            var product = new Product();
            PartialViewResult add = _productVariantController.Add(product);

            A.CallTo(
                () => _productVariantAdminService.SetViewData(_productVariantController.ViewData, A<ProductVariant>._))
                .MustHaveHappened();
        }

        [Fact]
        public void ProductVariantController_AddPost_CallsAddProductVariantService()
        {
            ProductVariant productVariant = GetNewProductVariant();
            ActionResult addPost = _productVariantController.Add_POST(productVariant);

            A.CallTo(() => _productVariantAdminService.Add(productVariant)).MustHaveHappened();
        }

        [Fact]
        public void ProductVariantController_AddPost_ReturnsARedirectToRouteResult()
        {
            ProductVariant productVariant = GetNewProductVariant();
            ActionResult addPost = _productVariantController.Add_POST(productVariant);

            addPost.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductVariantController_Edit_ReturnsAPartialViewResult()
        {
            ProductVariant productVariant = GetNewProductVariant();

            PartialViewResult edit = _productVariantController.Edit(productVariant);

            edit.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductVariantController_Edit_ShouldReturnThePassedProductVariantAsTheModel()
        {
            ProductVariant productVariant = GetNewProductVariant();

            PartialViewResult edit = _productVariantController.Edit(productVariant);

            edit.Model.Should().Be(productVariant);
        }

        [Fact]
        public void ProductVariantController_Edit_ViewDataTaxRatesTaxRateOptions()
        {
            ProductVariant productVariant = GetNewProductVariant();
            PartialViewResult edit = _productVariantController.Edit(productVariant);

            A.CallTo(
                () => _productVariantAdminService.SetViewData(_productVariantController.ViewData, productVariant))
                .MustHaveHappened();
        }

        [Fact]
        public void ProductVariantController_EditPost_ShouldCallUpdateMethodOfService()
        {
            ProductVariant productVariant = GetNewProductVariant();

            ActionResult edit = _productVariantController.Edit_POST(productVariant);

            A.CallTo(() => _productVariantAdminService.Update(productVariant)).MustHaveHappened();
        }

        [Fact]
        public void ProductVariantController_EditPost_ShouldReturnRedirectToRouteResult()
        {
            ProductVariant productVariant = GetNewProductVariant();

            ActionResult edit = _productVariantController.Edit_POST(productVariant);

            edit.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductVariantController_Delete_ShouldReturnAPartialViewResult()
        {
            ProductVariant productVariant = GetNewProductVariant();

            PartialViewResult delete = _productVariantController.Delete(productVariant);

            delete.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void ProductVariantController_Delete_ShouldReturnThePassedModel()
        {
            ProductVariant productVariant = GetNewProductVariant();

            PartialViewResult delete = _productVariantController.Delete(productVariant);

            delete.Model.Should().Be(productVariant);
        }

        [Fact]
        public void ProductVariantController_DeletePOST_ShouldReturnARedirectToRouteResult()
        {
            ProductVariant productVariant = GetNewProductVariant();

            RedirectToRouteResult delete = _productVariantController.Delete_POST(productVariant);

            delete.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void ProductVariantController_DeletePOST_ShouldCallDeleteOnService()
        {
            ProductVariant productVariant = GetNewProductVariant();

            RedirectToRouteResult delete = _productVariantController.Delete_POST(productVariant);

            A.CallTo(() => _productVariantAdminService.Delete(productVariant)).MustHaveHappened();
        }

        [Fact]
        public void ProductVariantController_DeletePOST_ShouldRedirectToEditProduct()
        {
            ProductVariant productVariant = GetNewProductVariant();

            RedirectToRouteResult delete = _productVariantController.Delete_POST(productVariant);

            delete.RouteValues["action"].Should().Be("Edit");
            delete.RouteValues["controller"].Should().Be("Webpage");
            delete.RouteValues["id"].Should().Be(123);
        }

        private static ProductVariant GetNewProductVariant()
        {
            return new ProductVariant {Product = new Product {Id = 123}};
        }
    }
}