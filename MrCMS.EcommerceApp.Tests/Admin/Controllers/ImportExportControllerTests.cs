using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using MrCMS.EcommerceApp.Tests.Stubs;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using Xunit;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class ImportExportControllerTests : InMemoryDatabaseTest
    {
        private readonly IImportExportManager _importExportManager;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly GoogleBaseSettings _googleBaseSettings;
        private readonly IGoogleBaseService _googleBaseService;
        private readonly IProductVariantService _productVariantService;
        private readonly IOptionService _optionService;
        private readonly ImportExportController _importExportController;

        public ImportExportControllerTests()
        {
            _importExportManager = A.Fake<IImportExportManager>();
            _configurationProvider = A.Fake<IConfigurationProvider>();
            _googleBaseSettings = A.Fake<GoogleBaseSettings>();
            _googleBaseService = A.Fake<IGoogleBaseService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _optionService = A.Fake<IOptionService>();

            _importExportController = new ImportExportController(_importExportManager,
                _configurationProvider, _googleBaseSettings, _optionService,_googleBaseService
                ,_productVariantService);
        }

        [Fact]
        public void ImportExportController_Products_ShouldReturnViewResult()
        {
            var result = _importExportController.Products();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ImportExportController_ExportProducts_ShouldReturnFileContentResult()
        {
            var result = _importExportController.ExportProducts();

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void ImportExportController_ImportProducts_ShouldReturnViewResult()
        {
            var file = A.Fake<HttpPostedFileBase>();

            var result = _importExportController.ImportProducts(file);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ImportExportController_ImportProducts_ShouldCallImportProductsFromExcelOfImportExportManager()
        {
            var file = new BasicHttpPostedFileBase();

            A.CallTo(() => _importExportManager.ImportProductsFromExcel(file.InputStream))
             .Returns(new Dictionary<string, List<string>>());
    
            var result = _importExportController.ImportProducts(file);

            AssertionExtensions.Should((object)AssertionExtensions.As<Dictionary<string, List<string>>>(result.ViewBag.Messages)).NotBeNull();
        }

        [Fact]
        public void ImportExportController_GoogleBase_ShouldReturnViewResult()
        {
            var model = new GoogleBaseModel();

            var result = _importExportController.GoogleBase(model);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ImportExportController_GoogleBase_ShouldReturnNonEmptyModel()
        {
            var model = new GoogleBaseModel();

            A.CallTo(() => _productVariantService.GetAllVariants(string.Empty,0,1))
            .Returns(new PagedList<ProductVariant>(new List<ProductVariant>(),1,10));

            var result = _importExportController.GoogleBase(model);

            result.Model.Should().NotBeNull();
        }

        [Fact]
        public void ImportExportController_GoogleBase_ShouldCall5ServicesToGetOptions()
        {
            var model = new GoogleBaseModel();

            A.CallTo(() => _productVariantService.GetAllVariants(string.Empty, 0, 1))
            .Returns(new PagedList<ProductVariant>(new List<ProductVariant>(), 1, 10));

            _importExportController.GoogleBase(model);

            A.CallTo(() => _googleBaseService.GetGoogleCategories()).MustHaveHappened();
            A.CallTo(() => _optionService.GetEnumOptions<ProductCondition>()).MustHaveHappened();
            A.CallTo(() => _optionService.GetCategoryOptions()).MustHaveHappened();
            A.CallTo(() => _optionService.GetEnumOptions<Gender>()).MustHaveHappened();
            A.CallTo(() => _optionService.GetEnumOptions<AgeGroup>()).MustHaveHappened();
        }

        [Fact]
        public void ImportExportController_ExportProductsToGoogleBase_ShouldReturnFileContentResult()
        {
            var result = _importExportController.ExportProductsToGoogleBase();

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void ImportExportController_ExportProductsToGoogleBase_ShouldCallExportProductsToGoogleBaseOfImportExportManager()
        {
            _importExportController.ExportProductsToGoogleBase();

            A.CallTo(() => _importExportManager.ExportProductsToGoogleBase()).MustHaveHappened();
        }

        [Fact]
        public void ImportExportController_GoogleBaseSettings_ShouldCallSaveSettingsOfConfigurationProvider()
        {
            var settings = new GoogleBaseSettings();

            _importExportController.GoogleBaseSettings(settings);

            A.CallTo(() => _configurationProvider.SaveSettings(settings)).MustHaveHappened();
        }
        /*
        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldReturnJsonResult()
        {
            var gbp = new GoogleBaseProduct()
            {
                OverrideCondition = ProductCondition.New,
                Gender = "Female",
                AgeGroup = "Male"
            };
            var pv = new ProductVariant();

            var result = _importExportController.UpdateGoogleBaseProduct(pv, gbp);

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldReturnTrueIfAllNecessaryValuesAreForwarded()
        {
            var gbp = new GoogleBaseProduct()
            {
                OverrideCondition = ProductCondition.New,
                Gender = "Female",
                AgeGroup = "Male"
            };
            var pv = new ProductVariant();

            var result = _importExportController.UpdateGoogleBaseProduct(pv, gbp);
            result.Data.Should().Be(true);
        }

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldReturnFalseIfProductVariantIsNotForwarded()
        {
            var result = _importExportController.UpdateGoogleBaseProduct(null,null);

            result.Data.Should().Be(false);
        }

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldCallUpdateGoogleBaseProductAndVariant()
        {
            var gbp = new GoogleBaseProduct()
            {
                Id=1,
                OverrideCondition = ProductCondition.New,
                Gender = "Female",
                AgeGroup = "Male"
            };
            var pv = new ProductVariant()
            {
                GoogleBaseProduct = gbp
            };

            A.CallTo(() => _googleBaseService.GetGoogleBaseProduct(1)).Returns(gbp);

            _importExportController.UpdateGoogleBaseProduct(pv, gbp);

            A.CallTo(() => _googleBaseService.UpdateGoogleBaseProductAndVariant(gbp)).MustHaveHappened();
        }*/
    }
}