using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class AttributeValueTests : InMemoryDatabaseTest
    {
        private readonly IProductSearchIndexService _productSearchIndexService;
        private readonly ProductOptionManager _productOptionManager;

        public AttributeValueTests()
        {
            _productSearchIndexService = A.Fake<IProductSearchIndexService>();
            _productOptionManager = new ProductOptionManager(Session, _productSearchIndexService,
                A.Fake<IUniquePageService>());
        }

        [Fact]
        public void ProductVariantOptionManager_SetAttributeValue_DoesNothingIfAnOptionWithTheNameDoesNotExist()
        {
            var variant = CreateProductVariant("ProductVariant");

            _productOptionManager.SetAttributeValue(variant, "Test", "Value");

            Session.QueryOver<ProductOptionValue>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductVariantOptionManager_SetAttributeValue_CreatesValueIfOptionExists()
        {
            var variant = CreateProductVariant("ProductVariant");
            var option = CreateOption("Test");

            _productOptionManager.SetAttributeValue(variant, "Test", "Value");

            Session.QueryOver<ProductOptionValue>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductVariantOptionManager_SetAttributeValue_UpdatesAnExistingValueIfOneExists()
        {
            var variant = CreateProductVariant("ProductVariant");
            var option = CreateOption("Test");
            var value = CreateValue(variant, option, "Value");

            _productOptionManager.SetAttributeValue(variant, "Test", "Updated Value");

            Session.QueryOver<ProductOptionValue>().RowCount().Should().Be(1);
            Session.QueryOver<ProductOptionValue>().List().First().Value.Should().Be("Updated Value");
        }

        private ProductOptionValue CreateValue(ProductVariant product, ProductOption option, string value)
        {
            var specValue = new ProductOptionValue
            {
                ProductVariant = product,
                ProductOption = option,
                Value = value
            };
            Session.Transact(session => session.Save(specValue));
            return specValue;
        }

        private ProductOption CreateOption(string name)
        {
            var option = new ProductOption { Name = name };
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