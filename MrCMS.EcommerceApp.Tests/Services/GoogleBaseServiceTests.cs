using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class GoogleBaseServiceTests :InMemoryDatabaseTest
    {
        private readonly GoogleBaseService _googleBaseService;
        private readonly IProductVariantService _productVariantService;
        private readonly GoogleBaseSettings _googleBaseSettings;

        public GoogleBaseServiceTests()
        {
            _productVariantService = A.Fake<IProductVariantService>();
            _googleBaseSettings = A.Fake<GoogleBaseSettings>();
            _googleBaseService = new GoogleBaseService(Session, _productVariantService,_googleBaseSettings);
        }

        [Fact]
        public void GoogleBaseService_GetGoogleCategories_ShouldTypeListOfSelectListItems()
        {
            var result = _googleBaseService.GetGoogleCategories();

            result.Should().BeOfType<List<SelectListItem>>();
        }

        [Fact]
        public void GoogleBaseService_GetGoogleCategories_ShouldReturnAtLeastOneItem()
        {
            var result = _googleBaseService.GetGoogleCategories();

            result.Count.Should().BeGreaterThan(0);
        }
        /*
        [Fact]
        public void GoogleBaseService_Get_ShouldGetItem()
        {
            var pv = new ProductVariant();
            var item = new GoogleBaseProduct {Id=1, ProductVariant = pv };

            _googleBaseService.AddGoogleBaseProduct(item);

            var result=_googleBaseService.GetGoogleBaseProduct(item.Id);

            result.Should().NotBeNull();
        }

        [Fact]
        public void GoogleBaseService_AddGoogleBaseProduct_ShouldAddItemToSession()
        {
            var pv = new ProductVariant();
            var item = new GoogleBaseProduct {ProductVariant = pv};

            _googleBaseService.AddGoogleBaseProduct(item);

            Session.QueryOver<GoogleBaseProduct>().List().Should().HaveCount(1);
        }

        [Fact]
        public void GoogleBaseService_UpdateGoogleBaseProduct_ShouldUpdateItem()
        {
            var pv = new ProductVariant();
            var item = new GoogleBaseProduct { ProductVariant = pv };

            Session.Transact(session => session.Save(item));
            item.Grouping = "Group2";

            _googleBaseService.UpdateGoogleBaseProduct(item);
            Session.Evict(item);

            Session.QueryOver<GoogleBaseProduct>().SingleOrDefault().Grouping.Should().Be("Group2");
        }

        [Fact]
        public void GoogleBaseService_UpdateGoogleBaseProductAndVariant_ShouldCallUpdateOfProductVariantService()
        {
            var pv = new ProductVariant();
            var item = new GoogleBaseProduct { ProductVariant = pv };

            _googleBaseService.UpdateGoogleBaseProductAndVariant(item);

            A.CallTo(() => _productVariantService.Update(pv)).MustHaveHappened();
        }*/
    }
}
