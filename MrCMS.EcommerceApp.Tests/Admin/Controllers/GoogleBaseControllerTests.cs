using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class GoogleBaseControllerTests : InMemoryDatabaseTest
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly GoogleBaseSettings _googleBaseSettings;
        private readonly IGoogleBaseManager _googleBaseManager;
        private readonly IProductVariantService _productVariantService;
        private readonly IOptionService _optionService;
        private readonly GoogleBaseController _googleBaseController;

        public GoogleBaseControllerTests()
        {
            _configurationProvider = A.Fake<IConfigurationProvider>();
            _googleBaseSettings = A.Fake<GoogleBaseSettings>();
            _googleBaseManager = A.Fake<IGoogleBaseManager>();
            _productVariantService = A.Fake<IProductVariantService>();
            _optionService = A.Fake<IOptionService>();

            _googleBaseController = new GoogleBaseController(_configurationProvider, _googleBaseSettings, _optionService,
                _productVariantService,_googleBaseManager);
        }

        [Fact]
        public void GoogleBaseController_Dashboard_ShouldReturnViewResult()
        {
            var model = new GoogleBaseModel();

            var result = _googleBaseController.Dashboard(model);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void GoogleBaseController_Dashboard_ShouldReturnNonEmptyModel()
        {
            var model = new GoogleBaseModel();

            A.CallTo(() => _productVariantService.GetAllVariants(string.Empty,0,1))
            .Returns(new PagedList<ProductVariant>(new List<ProductVariant>(),1,10));

            var result = _googleBaseController.Dashboard(model);

            result.Model.Should().NotBeNull();
        }

        [Fact]
        public void GoogleBaseController_Dashboard_ShouldCall5ServicesToGetOptions()
        {
            var model = new GoogleBaseModel();

            A.CallTo(() => _productVariantService.GetAllVariants(string.Empty, 0, 1))
            .Returns(new PagedList<ProductVariant>(new List<ProductVariant>(), 1, 10));

            _googleBaseController.Dashboard(model);

            A.CallTo(() => _optionService.GetEnumOptions<ProductCondition>()).MustHaveHappened();
            A.CallTo(() => _optionService.GetCategoryOptions()).MustHaveHappened();
            A.CallTo(() => _optionService.GetEnumOptions<Gender>()).MustHaveHappened();
            A.CallTo(() => _optionService.GetEnumOptions<AgeGroup>()).MustHaveHappened();
        }

        [Fact]
        public void GoogleBaseController_ExportProductsToGoogleBase_ShouldReturnFileContentResult()
        {
            var result = _googleBaseController.ExportProductsToGoogleBase();

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void GoogleBaseController_ExportProductsToGoogleBase_ShouldCallExportProductsToGoogleBaseOfGoogleBaseManager()
        {
            _googleBaseController.ExportProductsToGoogleBase();

            A.CallTo(() => _googleBaseManager.ExportProductsToGoogleBase()).MustHaveHappened();
        }

        [Fact]
        public void GoogleBaseController_GoogleBaseSettings_ShouldCallSaveSettingsOfConfigurationProvider()
        {
            var settings = new GoogleBaseSettings();

            _googleBaseController.GoogleBaseSettings(settings);

            A.CallTo(() => _configurationProvider.SaveSettings(settings)).MustHaveHappened();
        }

    }
}