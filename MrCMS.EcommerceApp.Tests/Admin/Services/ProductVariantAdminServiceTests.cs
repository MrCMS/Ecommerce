using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Services
{
    public class ProductVariantAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly ProductVariantAdminService _service;
        private IProductVariantAdminViewDataService _productVariantAdminViewDataService;

        public ProductVariantAdminServiceTests()
        {
            _productVariantAdminViewDataService = A.Fake<IProductVariantAdminViewDataService>();
            _service = new ProductVariantAdminService(_productVariantAdminViewDataService, Session);
        }

        [Fact]
        public void Add_ShouldPersistToTheSession()
        {
            var product = new Product();
            var productVariant = new ProductVariant {Product = product};
            _service.Add(productVariant);

            Session.QueryOver<ProductVariant>().RowCount().Should().Be(1);
        }

        [Fact]
        public void Add_ShouldAddVariantToTheProductsVariants()
        {
            var product = new Product();
            var productVariant = new ProductVariant {Product = product};
            _service.Add(productVariant);

            product.Variants.Should().Contain(productVariant);
        }

        [Fact]
        public void Edit_ShouldUpdateAVariant()
        {
            var productVariant = new ProductVariant();
            Session.Transact(session => session.Save(productVariant));
            productVariant.SKU = "updated";

            _service.Update(productVariant);
            Session.Evict(productVariant);
            Session.Get<ProductVariant>(1).SKU.Should().Be("updated");
        }

        [Fact]
        public void Delete_ShouldDeleteFromSession()
        {
            var productVariant = new ProductVariant();
            Session.Transact(session => session.Save(productVariant));
            _service.Delete(productVariant);

            Session.QueryOver<ProductVariant>().RowCount().Should().Be(0);
        }
    }
}