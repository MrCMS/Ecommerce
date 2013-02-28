using System.Linq;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Services.ProductOptionManagerTests
{
    public class AttributeOptionTests : InMemoryDatabaseTest
    {
        [Fact]
        public void ProductOptionManager_AddAttributeOption_ShouldSaveOption()
        {
            var productOptionManager = GetProductOptionManager();

            productOptionManager.AddAttributeOption(new ProductAttributeOption { Name = "Test" });

            Session.QueryOver<ProductAttributeOption>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductOptionManager_AddAttributeOption_DoesNotAllowAddingAnotherOptionWithSameName()
        {
            var productOptionManager = GetProductOptionManager();
            Session.Transact(session => session.Save(new ProductAttributeOption { Name = "Test" }));

            productOptionManager.AddAttributeOption(new ProductAttributeOption { Name = "Test" });

            Session.QueryOver<ProductAttributeOption>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductOptionManager_AddAttributeOption_DoesNotAllowAddingAnOptionWithNoName()
        {
            var productOptionManager = GetProductOptionManager();

            productOptionManager.AddAttributeOption(new ProductAttributeOption { Name = "" });

            Session.QueryOver<ProductAttributeOption>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_AllowsNameToBeUpdated()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductAttributeOption { Name = "Test" };
            Session.Transact(session => session.Save(option));
            option.Name = "Updated";

            productOptionManager.UpdateAttributeOptionn(option);

            Session.Evict(option);
            Session.Get<ProductAttributeOption>(1).Name.Should().Be("Updated");
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_DoesNotAllowNameToBeSameAsAnExistingOption()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductAttributeOption { Name = "Test" };
            Session.Transact(session => session.Save(option));
            var option2 = new ProductAttributeOption { Name = "Test 2" };
            Session.Transact(session => session.Save(option2));
            option.Name = "Test 2";

            productOptionManager.UpdateAttributeOptionn(option);

            Session.Evict(option);
            Session.Get<ProductAttributeOption>(1).Name.Should().Be("Test");
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_DoesNotAllowNameToBeAnEmptyString()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductAttributeOption { Name = "Test" };
            Session.Transact(session => session.Save(option));
            option.Name = "";

            productOptionManager.UpdateAttributeOptionn(option);

            Session.Evict(option);
            Session.Get<ProductAttributeOption>(1).Name.Should().Be("Test");
        }

        [Fact]
        public void ProductOptionManager_ListAttributeOptions_ReturnsAllAttributeOptions()
        {
            var productOptionManager = GetProductOptionManager();
            var options = Enumerable.Range(1, 10).Select(i => new ProductAttributeOption()).ToList();
            Session.Transact(session => options.ForEach(option => session.Save(option)));

            var listAttributeOptions = productOptionManager.ListAttributeOptions();

            listAttributeOptions.Should().HaveCount(10);
        }

        [Fact]
        public void ProductOptionManager_DeleteAttributeOption_DeletesOption()
        {
            var productOptionManager = GetProductOptionManager();
            var option = new ProductAttributeOption { Name = "Test" };
            Session.Transact(session => session.Save(option));

            productOptionManager.DeleteAttributeOption(option);

            Session.QueryOver<ProductAttributeOption>().RowCount().Should().Be(0);
        }

        private static ProductOptionManager GetProductOptionManager()
        {
            return new ProductOptionManager(Session);
        }
    }
}