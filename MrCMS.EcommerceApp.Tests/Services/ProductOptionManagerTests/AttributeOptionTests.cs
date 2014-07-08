using System.Collections.Generic;
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
    public class AttributeOptionTests : InMemoryDatabaseTest
    {
        private readonly ProductOptionManager _productOptionManager;
        private readonly IProductSearchIndexService _productSearchIndexService;

        public AttributeOptionTests()
        {
            _productSearchIndexService = A.Fake<IProductSearchIndexService>();
            _productOptionManager = new ProductOptionManager(Session, _productSearchIndexService,
                A.Fake<IUniquePageService>());
        }

        [Fact]
        public void ProductOptionManager_AddAttributeOption_ShouldSaveOption()
        {
            _productOptionManager.AddAttributeOption(new ProductOption {Name = "Test"});

            Session.QueryOver<ProductOption>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductOptionManager_AddAttributeOption_DoesNotAllowAddingAnotherOptionWithSameName()
        {
            Session.Transact(session => session.Save(new ProductOption {Name = "Test"}));

            _productOptionManager.AddAttributeOption(new ProductOption {Name = "Test"});

            Session.QueryOver<ProductOption>().RowCount().Should().Be(1);
        }

        [Fact]
        public void ProductOptionManager_AddAttributeOption_DoesNotAllowAddingAnOptionWithNoName()
        {
            _productOptionManager.AddAttributeOption(new ProductOption {Name = ""});

            Session.QueryOver<ProductOption>().RowCount().Should().Be(0);
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_AllowsNameToBeUpdated()
        {
            var option = new ProductOption {Name = "Test"};
            Session.Transact(session => session.Save(option));
            var id = option.Id;
            option.Name = "Updated";

            _productOptionManager.UpdateAttributeOption(option);

            Session.Evict(option);
            Session.QueryOver<ProductOption>().Where(productOption => productOption.Id == id).SingleOrDefault().Name.Should().Be("Updated");
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_DoesNotAllowNameToBeSameAsAnExistingOption()
        {
            var option = new ProductOption {Name = "Test"};
            Session.Transact(session => session.Save(option));
            var id = option.Id;
            var option2 = new ProductOption {Name = "Test 2"};
            Session.Transact(session => session.Save(option2));
            option.Name = "Test 2";

            _productOptionManager.UpdateAttributeOption(option);

            Session.Evict(option);
            var name = Session.QueryOver<ProductOption>().Where(productOption => productOption.Id==id).SingleOrDefault().Name;
            name.Should().Be("Test", "because {0} is wrong", name);
        }

        [Fact]
        public void ProductOptionManager_UpdateAttributeOption_DoesNotAllowNameToBeAnEmptyString()
        {
            var option = new ProductOption {Name = "Test"};
            Session.Transact(session => session.Save(option));
            var id = option.Id;
            option.Name = "";

            _productOptionManager.UpdateAttributeOption(option);

            Session.Evict(option);
            var name = Session.QueryOver<ProductOption>().Where(productOption => productOption.Id == id).SingleOrDefault().Name;
            name.Should().Be("Test", "because {0} is wrong", name);
        }

        [Fact]
        public void ProductOptionManager_ListAttributeOptions_ReturnsAllAttributeOptions()
        {
            List<ProductOption> options = Enumerable.Range(1, 10).Select(i => new ProductOption()).ToList();
            Session.Transact(session => options.ForEach(option => session.Save(option)));

            IList<ProductOption> listAttributeOptions = _productOptionManager.ListAttributeOptions();

            listAttributeOptions.Should().HaveCount(10);
        }

        [Fact]
        public void ProductOptionManager_DeleteAttributeOption_DeletesOption()
        {
            var option = new ProductOption {Name = "Test"};
            Session.Transact(session => session.Save(option));

            _productOptionManager.DeleteAttributeOption(option);

            Session.QueryOver<ProductOption>().RowCount().Should().Be(0);
        }
    }
}