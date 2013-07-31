using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using MrCMS.EcommerceApp.Tests.Stubs;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Services.Users;
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
        private readonly IProductConditionService _productConditionService;
        private readonly ICategoryService _categoryService;
        private readonly IGoogleBaseTaxonomyService _googleBaseTaxonomyService;
        private readonly IProductVariantService _productVariantService;
        private readonly IGenderService _genderService;
        private readonly IAgeGroupService _ageGroupService;
        private readonly IGoogleBaseProductService _googleBaseProductService;
        private readonly ImportExportController _importExportController;

        public ImportExportControllerTests()
        {
            _importExportManager = A.Fake<IImportExportManager>();
            _configurationProvider = A.Fake<IConfigurationProvider>();
            _googleBaseSettings = A.Fake<GoogleBaseSettings>();
            _productConditionService = A.Fake<IProductConditionService>();
            _categoryService = A.Fake<ICategoryService>();
            _googleBaseTaxonomyService = A.Fake<IGoogleBaseTaxonomyService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _genderService = A.Fake<IGenderService>();
            _ageGroupService = A.Fake<IAgeGroupService>();
            _googleBaseProductService = A.Fake<IGoogleBaseProductService>();

            _importExportController = new ImportExportController(_importExportManager,
                _configurationProvider, _googleBaseSettings, _productConditionService,
                _categoryService, _googleBaseTaxonomyService, _productVariantService,
                _genderService, _ageGroupService, _googleBaseProductService);
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

            A.CallTo(() => _googleBaseTaxonomyService.GetOptions()).MustHaveHappened();
            A.CallTo(() => _productConditionService.GetOptions()).MustHaveHappened();
            A.CallTo(() => _categoryService.GetOptions()).MustHaveHappened();
            A.CallTo(() => _genderService.GetOptions()).MustHaveHappened();
            A.CallTo(() => _ageGroupService.GetOptions()).MustHaveHappened();
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

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldReturnJsonResult()
        {
            var pv = new ProductVariant();

            var result = _importExportController.UpdateGoogleBaseRecord(pv,"Cat","Gro","Lab","Red",0,ProductCondition.New,Gender.Female,AgeGroup.Kids);

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldReturnTrueIfAllNecessaryValuesAreForwarded()
        {
            var pv = new ProductVariant();

            var result = _importExportController.UpdateGoogleBaseRecord(pv, "Cat", "Gro", "Lab", "Red", 0, ProductCondition.New, Gender.Female, AgeGroup.Kids);

            result.Data.Should().Be(true);
        }

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldReturnFalseIfProductVariantIsNotForwarded()
        {
            var result = _importExportController.UpdateGoogleBaseRecord(null, "Cat", "Gro", "Lab", "Red", 0, ProductCondition.New, Gender.Female, AgeGroup.Kids);

            result.Data.Should().Be(false);
        }

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldCallUpdateOfProductVariantService()
        {
            var pv = new ProductVariant();

            _importExportController.UpdateGoogleBaseRecord(pv, "Cat", "Gro", "Lab", "Red", 0, ProductCondition.New, Gender.Female, AgeGroup.Kids);

            A.CallTo(() => _productVariantService.Update(pv)).MustHaveHappened();
        }

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldCallAddOfGoogleBaseProductService()
        {
            var gbp = new GoogleBaseProduct()
                {
                    OverrideCondition = ProductCondition.New,
                    Gender = Gender.Female,
                    AgeGroup = AgeGroup.Kids
                };
            var pv = new ProductVariant()
                {
                    GoogleBaseProduct = gbp
                };

            A.CallTo(() => _googleBaseProductService.Get(0)).Returns(gbp);

            _importExportController.UpdateGoogleBaseRecord(pv, null, null, null,null, 0, ProductCondition.New, Gender.Female, AgeGroup.Kids);

            A.CallTo(() => _googleBaseProductService.Add(gbp)).MustHaveHappened();
        }

        [Fact]
        public void ImportExportController_UpdateGoogleBaseRecord_ShouldCallUpdateOfGoogleBaseProductServiceIfGBPAlreadyExists()
        {
            var gbp = new GoogleBaseProduct()
            {
                Id=1,
                OverrideCondition = ProductCondition.New,
                Gender = Gender.Female,
                AgeGroup = AgeGroup.Kids
            };
            var pv = new ProductVariant()
            {
                GoogleBaseProduct = gbp
            };

            A.CallTo(() => _googleBaseProductService.Get(1)).Returns(gbp);

            _importExportController.UpdateGoogleBaseRecord(pv, null, null, null, null, 1, ProductCondition.New, Gender.Female, AgeGroup.Kids);

            A.CallTo(() => _googleBaseProductService.Update(gbp)).MustHaveHappened();
        }
    }
}