using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class AttributeOptionTests : InMemoryDatabaseTest
    {
        private readonly IProductSearchService _productSearchService;
        private readonly ProductOptionManager _productOptionManager;

        public AttributeOptionTests()
        {
            _productSearchService = A.Fake<IProductSearchService>();
            _productOptionManager = new ProductOptionManager(Session, _productSearchService);
        }
        [Fact]
        public void ProductOptionManager_AddAttributeOption_ShouldSaveOption()
        {
            _productOptionManager.AddAttributeOption(new ProductAttributeOption { Name = "Test" });

            Session.QueryOver<ProductAttributeOption>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductOptionManager_AddAttributeOption_DoesNotAllowAddingAnotherOptionWithSameName()
        {
            Session.Transact(session => session.Save(new ProductAttributeOption { Name = "Test" }));

            _productOptionManager.AddAttributeOption(new ProductAttributeOption { Name = "Test" });

            Session.QueryOver<ProductAttributeOption>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductOptionManager_AddAttributeOption_DoesNotAllowAddingAnOptionWithNoName()
        {
            _productOptionManager.AddAttributeOption(new ProductAttributeOption { Name = "" });

            Session.QueryOver<ProductAttributeOption>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_AllowsNameToBeUpdated()
        {
            var option = new ProductAttributeOption { Name = "Test" };
            Session.Transact(session => session.Save(option));
            option.Name = "Updated";

            _productOptionManager.UpdateAttributeOption(option);

            Session.Evict(option);
            Session.Get<ProductAttributeOption>(1).Name.Should().Be("Updated");
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_DoesNotAllowNameToBeSameAsAnExistingOption()
        {
            var option = new ProductAttributeOption { Name = "Test" };
            Session.Transact(session => session.Save(option));
            var option2 = new ProductAttributeOption { Name = "Test 2" };
            Session.Transact(session => session.Save(option2));
            option.Name = "Test 2";

            _productOptionManager.UpdateAttributeOption(option);

            Session.Evict(option);
            Session.Get<ProductAttributeOption>(1).Name.Should().Be("Test");
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_DoesNotAllowNameToBeAnEmptyString()
        {
            var option = new ProductAttributeOption { Name = "Test" };
            Session.Transact(session => session.Save(option));
            option.Name = "";

            _productOptionManager.UpdateAttributeOption(option);

            Session.Evict(option);
            Session.Get<ProductAttributeOption>(1).Name.Should().Be("Test");
        }

        [Fact]
        public void ProductOptionManager_ListAttributeOptions_ReturnsAllAttributeOptions()
        {
            var options = Enumerable.Range(1, 10).Select(i => new ProductAttributeOption()).ToList();
            Session.Transact(session => options.ForEach(option => session.Save(option)));

            var listAttributeOptions = _productOptionManager.ListAttributeOptions();

            listAttributeOptions.Should().HaveCount(10);
        }

        [Fact]
        public void ProductOptionManager_DeleteAttributeOption_DeletesOption()
        {
            var option = new ProductAttributeOption { Name = "Test" };
            Session.Transact(session => session.Save(option));

            _productOptionManager.DeleteAttributeOption(option);

            Session.QueryOver<ProductAttributeOption>().RowCount().Should().Be(0);
        }
    }
}