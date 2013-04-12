using System.Linq;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class SpecificationOptionTests : InMemoryDatabaseTest
    {
        [Fact]
        public void ProductOptionManager_AddSpecificationOption_SavesOption()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationOption { Name = "test" };

            productOptionManager.AddSpecificationOption(option);

            Session.QueryOver<ProductSpecificationOption>().List().Should().HaveCount(1);
        }

        [Fact]
        public void ProductOptionManager_AddSpecificationOption_DoesNotSaveIfExistingOptionWithSameName()
        {
            var productOptionManager = GetProductOptionManager();
            var option1 = new ProductSpecificationOption { Name = "Test" };
            Session.Transact(session => session.Save(option1));

            productOptionManager.AddSpecificationOption(new ProductSpecificationOption { Name = "Test" });

            Session.QueryOver<ProductSpecificationOption>().List().Should().HaveCount(1);
        }

        [Fact]
        public void ProductOptionManager_AddSpecificationOption_DoesNotSaveIfOptionNameIsEmpty()
        {
            var productOptionManager = GetProductOptionManager();

            productOptionManager.AddSpecificationOption(new ProductSpecificationOption { Name = "" });

            Session.QueryOver<ProductSpecificationOption>().List().Should().HaveCount(0);
        }

        [Fact]
        public void ProductOptionManager_UpdateSpecificationOption_AllowsNameToBeUpdated()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationOption { Name = "Test" };
            Session.Transact(session => session.Save(option));
            option.Name = "Updated";

            productOptionManager.UpdateSpecificationOption(option);

            Session.Evict(option);
            Session.Get<ProductSpecificationOption>(1).Name.Should().Be("Updated");
        }

        [Fact]
        public void ProductOptionManager_UpdateSpecificationOption_ShouldNotSaveChangesIfOptionAlreadyHasThatName()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationOption { Name = "Test" };
            var option2 = new ProductSpecificationOption { Name = "Updated" };
            Session.Transact(session =>
                                 {
                                     session.Save(option);
                                     session.Save(option2);
                                 });
            option.Name = "Updated";

            productOptionManager.UpdateSpecificationOption(option);

            Session.Evict(option);
            Session.Get<ProductSpecificationOption>(1).Name.Should().Be("Test");
        }

        [Fact]
        public void ProductOptionManager_ListSpecificationOptions_ShouldReturnAllSavedSpecifications()
        {
            var productOptionManager = GetProductOptionManager();
            var productSpecificationOptions =
                Enumerable.Range(1, 10).Select(i => new ProductSpecificationOption { Name = "Test " + i }).ToList();
            Session.Transact(session => productSpecificationOptions.ForEach(option => session.Save(option)));

            var listSpecificationOptions = productOptionManager.ListSpecificationOptions();

            AssertionExtensions.ShouldAllBeEquivalentTo<ProductSpecificationOption>(listSpecificationOptions, productSpecificationOptions);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationOption_DeletesOption()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationOption { Name = "Test" };
            Session.Transact(session => session.Save(option));

            productOptionManager.DeleteSpecificationOption(option);

            Session.QueryOver<ProductSpecificationOption>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationOption_ShouldRemoveAllValues()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationOption { Name = "Test" };
            var product = new Product();
            var productSpecificationOptions =
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
            option.Values = productSpecificationOptions;
            Session.Transact(session =>
                                 {
                                     session.Save(product);
                                     session.Save(option);
                                     productSpecificationOptions.ForEach(value => session.Save(value));
                                 });

            productOptionManager.DeleteSpecificationOption(option);

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationOption_ShouldLeaveProductIntact()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductSpecificationOption { Name = "Test" };
            var product = new Product();
            var productSpecificationOptions =
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
            option.Values = productSpecificationOptions;
            Session.Transact(session =>
                                 {
                                     session.Save(product);
                                     session.Save(option);
                                     productSpecificationOptions.ForEach(value => session.Save(value));
                                 });

            productOptionManager.DeleteSpecificationOption(option);

            Session.QueryOver<Product>().RowCount().Should().Be(1);
        }

        private static ProductOptionManager GetProductOptionManager()
        {
            return new ProductOptionManager(Session);
        }
    }
}