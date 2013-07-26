using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class SpecificationAttributeTests : InMemoryDatabaseTest
    {
        private readonly IProductSearchService _productSearchService;
        private readonly ProductOptionManager _productOptionManager;

        public SpecificationAttributeTests()
        {
            _productSearchService = A.Fake<IProductSearchService>();
            _productOptionManager = new ProductOptionManager(Session, _productSearchService);
        }
        [Fact]
        public void ProductOptionManager_AddSpecificationAttribute_SavesOption()
        {
            var option = new ProductSpecificationAttribute { Name = "test" };

            _productOptionManager.AddSpecificationAttribute(option);

            Session.QueryOver<ProductSpecificationAttribute>().List().Should().HaveCount(1);
        }

        [Fact]
        public void ProductOptionManager_AddSpecificationAttribute_DoesNotSaveIfOptionNameIsEmpty()
        {
            _productOptionManager.AddSpecificationAttribute(new ProductSpecificationAttribute { Name = "" });

            Session.QueryOver<ProductSpecificationAttribute>().List().Should().HaveCount(0);
        }

        [Fact]
        public void ProductOptionManager_UpdateSpecificationAttribute_AllowsNameToBeUpdated()
        {
            var option = new ProductSpecificationAttribute { Name = "Test" };
            Session.Transact(session => session.Save(option));
            option.Name = "Updated";

            _productOptionManager.UpdateSpecificationAttribute(option);

            Session.Evict(option);
            Session.Get<ProductSpecificationAttribute>(1).Name.Should().Be("Updated");
        }

        [Fact]
        public void ProductOptionManager_ListSpecificationAttributes_ShouldReturnAllSavedSpecifications()
        {
            var productSpecificationAttributes =
                Enumerable.Range(1, 10).Select(i => new ProductSpecificationAttribute { Name = "Test " + i }).ToList();
            Session.Transact(session => productSpecificationAttributes.ForEach(option => session.Save(option)));

            var listSpecificationAttributes = _productOptionManager.ListSpecificationAttributes();

            listSpecificationAttributes.ShouldAllBeEquivalentTo(productSpecificationAttributes);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationAttribute_DeletesOption()
        {
            var option = new ProductSpecificationAttribute { Name = "Test" };
            Session.Transact(session => session.Save(option));

            _productOptionManager.DeleteSpecificationAttribute(option);

            Session.QueryOver<ProductSpecificationAttribute>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationAttribute_ShouldRemoveAllValues()
        {
            var attribute = new ProductSpecificationAttribute { Name = "Test" };
            var option = new ProductSpecificationAttributeOption() { Name = "Test Value" };
            var product = new Product();
            var productSpecificationAttributes =
                Enumerable.Range(1, 10)
                          .Select(
                              i =>
                              new ProductSpecificationValue
                                  {
                                      ProductSpecificationAttributeOption = option,
                                      Product = product
                                  })
                          .ToList();
            option.Values = productSpecificationAttributes;
            Session.Transact(session =>
                                 {
                                     session.Save(product);
                                     session.Save(attribute);
                                     productSpecificationAttributes.ForEach(value => session.Save(value));
                                 });

            _productOptionManager.DeleteSpecificationAttributeOption(option);

            Session.QueryOver<ProductSpecificationValue>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_DeleteSpecificationAttribute_ShouldLeaveProductIntact()
        {
            var attribute = new ProductSpecificationAttribute { Name = "Test" };
            var option = new ProductSpecificationAttributeOption() { Name = "Test Value" };
            var product = new Product();
            var productSpecificationAttributes =
                Enumerable.Range(1, 10)
                          .Select(
                              i =>
                              new ProductSpecificationValue
                              {
                                  ProductSpecificationAttributeOption = option,
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

            _productOptionManager.DeleteSpecificationAttribute(attribute);

            Session.QueryOver<Product>().RowCount().Should().Be(1);
        }
    }
}