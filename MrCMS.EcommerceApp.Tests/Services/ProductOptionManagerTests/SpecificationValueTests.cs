using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;
using MrCMS.Helpers;
using FluentAssertions;
using NHibernate.Util;
using System.Linq;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class SpecificationValueTests : InMemoryDatabaseTest
    {
        [Fact]
        public void ProductOptionManager_SetSpecificationValue_DoesNothingIfAnOptionWithTheNameDoesNotExist()
        {
            var productOptionManager = GetProductOptionManager();
            var product = CreateProduct("Product");

            productOptionManager.SetSpecificationValue(product, "Test", "Value");

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_SetSpecificationValue_CreatesValueIfOptionExists()
        {
            var productOptionManager = GetProductOptionManager();
            var product = CreateProduct("Product");
            var option = CreateOption("Test");

            productOptionManager.SetSpecificationValue(product, "Test", "Value");

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductOptionManager_SetSpecificationValue_UpdatesAnExistingValueIfOneExists()
        {
            var productOptionManager = GetProductOptionManager();
            var product = CreateProduct("Product");
            var option = CreateOption("Test");
            var value = CreateValue(product, option, "Value");

            productOptionManager.SetSpecificationValue(product, "Test", "Updated Value");

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(1);
            Session.QueryOver<ProductSpecificationValue>().List().First().Value.Should().Be("Updated Value");
        }

        private ProductSpecificationValue CreateValue(Product product, ProductSpecificationOption option, string value)
        {
            var specValue = new ProductSpecificationValue
                                {
                                    Product = product,
                                    Option = option,
                                    Value = value
                                };
            Session.Transact(session => session.Save(specValue));
            return specValue;
        }

        private ProductSpecificationOption CreateOption(string name)
        {
            var option = new ProductSpecificationOption {Name = name};
            Session.Transact(session => session.Save(option));
            return option;
        }

        private Product CreateProduct(string name)
        {
            var product = new Product {Name = name};
            Session.Transact(session => session.Save(product));
            return product;
        }

        private static ProductOptionManager GetProductOptionManager()
        {
            return new ProductOptionManager(Session);
        }
    }
}