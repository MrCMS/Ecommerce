using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Helpers;
using FluentAssertions;
using NHibernate.Util;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class SpecificationValueTests : InMemoryDatabaseTest
    {
        [Fact]
        public void ProductOptionManager_SetSpecificationValue_CreatesValueIfOptionExists()
        {
            var productOptionManager = GetProductOptionManager();
            var product = CreateProduct("Product");
            var option = CreateOption("Test");

            productOptionManager.SetSpecificationValue(product, option, "Value");

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductOptionManager_SetSpecificationValue_UpdatesAnExistingValueIfOneExists()
        {
            var productOptionManager = GetProductOptionManager();
            var product = CreateProduct("Product");
            var option = CreateOption("Test");
            var value = CreateValue(product, option, "Value");

            productOptionManager.SetSpecificationValue(product, option, "Updated Value");

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(1);
            Session.QueryOver<ProductSpecificationValue>().List().First().Value.Should().Be("Updated Value");
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationValue_ShouldRemoveSpecificationValue()
        {
            var productOptionManager = GetProductOptionManager();
            var product = CreateProduct("Product");
            var option = CreateOption("Size");
            var value = CreateValue(product, option, "11''");
            Session.Transact(session => session.Save(value));

            productOptionManager.DeleteSpecificationValue(value);

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_UpdateSpecificationValueDisplayOrder_ShouldChangeDisplayOrder()
        {
            var productOptionManager = GetProductOptionManager();
            var product = CreateProduct("Product");
            var option = CreateOption("Size");

            var value1 =  new ProductSpecificationValue
            {
                Product = product,
                Option = option,
                Value = "11"
            };
            var value2 = new ProductSpecificationValue
            {
                Product = product,
                Option = option,
                Value = "13"
            };

            product.SpecificationValues.Add(value1);
            product.SpecificationValues.Add(value2);

            Session.Transact(session => session.Save(option));
            Session.Transact(session => session.Save(product));
           
            var sortItems = product.SpecificationValues.OrderBy(x => x.DisplayOrder)
                                .Select(
                                    arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Option.Name })
                                .ToList();
            sortItems[0].Order = 0;
            sortItems[1].Order = 1;

            productOptionManager.UpdateSpecificationValueDisplayOrder(sortItems);

            Session.QueryOver<ProductSpecificationValue>().Where(x => x.DisplayOrder != 0).RowCount().Should().BeGreaterThan(0);
        }

        private ProductSpecificationValue CreateValue(Product product, ProductSpecificationAttribute option, string value)
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

        private ProductSpecificationAttribute CreateOption(string name)
        {
            var option = new ProductSpecificationAttribute {Name = name};
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