using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ProductVariantServiceTests : InMemoryDatabaseTest
    {
        private ProductVariantService _productVariantService;

        public ProductVariantServiceTests()
        {
            _productVariantService = new ProductVariantService(Session);
        }

        [Fact]
        public void ProductVariantService_Add_ShouldPersistToTheSession()
        {
            var product = new Product();
            var productVariant = new ProductVariant() { Product=product };

            _productVariantService.Add(productVariant);

            Session.QueryOver<ProductVariant>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductVariantService_Add_ShouldAddVariantToTheProductsVariants()
        {
            var product = new Product();
            var productVariant = new ProductVariant{Product=product};
            
            _productVariantService.Add(productVariant);

            product.Variants.Should().Contain(productVariant);
        }

        [Fact]
        public void ProductVariantService_Edit_ShouldUpdateAVariant()
        {
            var productVariant = new ProductVariant();
            Session.Transact(session => session.Save(productVariant));
            productVariant.SKU = "updated";

            _productVariantService.Update(productVariant);

            Session.Evict(productVariant);
            Session.Get<ProductVariant>(1).SKU.Should().Be("updated");
        }

        [Fact]
        public void ProductVariantService_Delete_ShouldDeleteFromSession()
        {
            var productVariant = new ProductVariant();
            Session.Transact(session => session.Save(productVariant));
            
            _productVariantService.Delete(productVariant);

            Session.QueryOver<ProductVariant>().RowCount().Should().Be(0);
        }
    }
}