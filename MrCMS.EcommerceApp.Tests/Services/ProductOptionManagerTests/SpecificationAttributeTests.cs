using System.Linq;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Helpers;
using MrCMS.Website;
using MrCMS.Entities.People;
using FakeItEasy;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class SpecificationAttributeTests : InMemoryDatabaseTest
    {
        [Fact]
        public void ProductOptionManager_AddSpecificationAttribute_SavesOption()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationAttribute { Name = "test" };

            productOptionManager.AddSpecificationAttribute(option);

            Session.QueryOver<ProductSpecificationAttribute>().List().Should().HaveCount(1);
        }

        [Fact]
        public void ProductOptionManager_AddSpecificationAttribute_DoesNotSaveIfOptionNameIsEmpty()
        {
            var productOptionManager = GetProductOptionManager();

            productOptionManager.AddSpecificationAttribute(new ProductSpecificationAttribute { Name = "" });

            Session.QueryOver<ProductSpecificationAttribute>().List().Should().HaveCount(0);
        }

        [Fact]
        public void ProductOptionManager_UpdateSpecificationAttribute_AllowsNameToBeUpdated()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationAttribute { Name = "Test" };
            Session.Transact(session => session.Save(option));
            option.Name = "Updated";

            productOptionManager.UpdateSpecificationAttribute(option);

            Session.Evict(option);
            Session.Get<ProductSpecificationAttribute>(1).Name.Should().Be("Updated");
        }

        [Fact]
        public void ProductOptionManager_ListSpecificationAttributes_ShouldReturnAllSavedSpecifications()
        {
            var productOptionManager = GetProductOptionManager();
            var productSpecificationAttributes =
                Enumerable.Range(1, 10).Select(i => new ProductSpecificationAttribute { Name = "Test " + i }).ToList();
            Session.Transact(session => productSpecificationAttributes.ForEach(option => session.Save(option)));

            var listSpecificationAttributes = productOptionManager.ListSpecificationAttributes();

            AssertionExtensions.ShouldAllBeEquivalentTo<ProductSpecificationAttribute>(listSpecificationAttributes, productSpecificationAttributes);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationAttribute_DeletesOption()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationAttribute { Name = "Test" };
            Session.Transact(session => session.Save(option));

            productOptionManager.DeleteSpecificationAttribute(option);

            Session.QueryOver<ProductSpecificationAttribute>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationAttribute_ShouldRemoveAllValues()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationAttribute { Name = "Test" };
            var product = new Product();
            var productSpecificationAttributes =
                Enumerable.Range(1, 10)
                          .Select(
                              i =>
                              new ProductSpecificationValue
                                  {
                                      Value = "Value" + i,
                                      Option = option,
                                      Product = product
                                  })
                          .ToList();
            option.Values = productSpecificationAttributes;
            Session.Transact(session =>
                                 {
                                     session.Save(product);
                                     session.Save(option);
                                     productSpecificationAttributes.ForEach(value => session.Save(value));
                                 });

            productOptionManager.DeleteSpecificationAttribute(option);

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationAttribute_ShouldLeaveProductIntact()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationAttribute { Name = "Test" };
            var product = new Product();
            var productSpecificationAttributes =
                Enumerable.Range(1, 10)
                          .Select(
                              i =>
                              new ProductSpecificationValue
                                  {
                                      Value = "Value" + i,
                                      Option = option,
                                      Product = product
                                  })
                          .ToList();
            option.Values = productSpecificationAttributes;
            Session.Transact(session =>
                                 {
                                     session.Save(product);
                                     session.Save(option);
                                     productSpecificationAttributes.ForEach(value => session.Save(value));
                                 });

            productOptionManager.DeleteSpecificationAttribute(option);

            Session.QueryOver<Product>().RowCount().Should().Be(1);
        }

        private static ProductOptionManager GetProductOptionManager()
        {
            return new ProductOptionManager(Session);
        }
    }
}