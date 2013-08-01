using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using FluentAssertions;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class OptionServiceTests :InMemoryDatabaseTest
    {
        private readonly OptionService _optionService;

        public OptionServiceTests()
        {
            _optionService = new OptionService(Session);
        }

        [Fact]
        public void OptionService_GetEnumOptions_ShouldTypeListOfSelectListItems()
        {
            var result=_optionService.GetEnumOptions<Gender>();

            result.Should().BeOfType<List<SelectListItem>>();
        }

        [Fact]
        public void OptionService_GetEnumOptions_ShouldReturnAtLeastOneItem()
        {
            var result = _optionService.GetEnumOptions<Gender>();

            result.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void OptionService_GetCategoryOptions_ShouldTypeListOfSelectListItems()
        {
            var result = _optionService.GetCategoryOptions();

            result.Should().BeOfType<List<SelectListItem>>();
        }

        [Fact]
        public void OptionService_GetCategoryOptions_ShouldReturnAtLeaseOneItem()
        {
            var category = new Category() {Name = "Cat"};
            Session.Transact(session => session.Save(category));

            var result = _optionService.GetCategoryOptions();

            result.Count.Should().BeGreaterThan(0);
        }
    }
}
