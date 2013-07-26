using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class AttributeValueTests : InMemoryDatabaseTest
    {
        private readonly IProductSearchService _productSearchService;
        private readonly ProductOptionManager _productOptionManager;

        public AttributeValueTests()
        {
            _productSearchService = A.Fake<IProductSearchService>();
            _productOptionManager = new ProductOptionManager(Session, _productSearchService);
        }

        [Fact]
        public void ProductVariantOptionManager_SetAttributeValue_DoesNothingIfAnOptionWithTheNameDoesNotExist()
        {
            var variant = CreateProductVariant("ProductVariant");

            _productOptionManager.SetAttributeValue(variant, "Test", "Value");

            Session.QueryOver<ProductAttributeValue>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductVariantOptionManager_SetAttributeValue_CreatesValueIfOptionExists()
        {
            var variant = CreateProductVariant("ProductVariant");
            var option = CreateOption("Test");

            _productOptionManager.SetAttributeValue(variant, "Test", "Value");

            Session.QueryOver<ProductAttributeValue>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductVariantOptionManager_SetAttributeValue_UpdatesAnExistingValueIfOneExists()
        {
            var variant = CreateProductVariant("ProductVariant");
            var option = CreateOption("Test");
            var value = CreateValue(variant, option, "Value");

            _productOptionManager.SetAttributeValue(variant, "Test", "Updated Value");

            Session.QueryOver<ProductAttributeValue>().RowCount().Should().Be(1);
            Session.QueryOver<ProductAttributeValue>().List().First().Value.Should().Be("Updated Value");
        }

        private ProductAttributeValue CreateValue(ProductVariant product, ProductAttributeOption option, string value)
        {
            var specValue = new ProductAttributeValue
            {
                ProductVariant = product,
                ProductAttributeOption = option,
                Value = value
            };
            Session.Transact(session => session.Save(specValue));
            return specValue;
        }

        private ProductAttributeOption CreateOption(string name)
        {
            var option = new ProductAttributeOption { Name = name };
            Session.Transact(session => session.Save(option));
            return option;
        }

        private ProductVariant CreateProductVariant(string name)
        {
            var product = new ProductVariant { SKU = name };
            Session.Transact(session => session.Save(product));
            return product;
        }
    }
}