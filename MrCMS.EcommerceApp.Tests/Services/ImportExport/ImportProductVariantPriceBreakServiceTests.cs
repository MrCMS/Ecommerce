using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Website;
using NHibernate;
using Ninject.MockingKernel;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ImportProductVariantPriceBreakServiceTests
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IImportProductVariantPriceBreaksService _importProductUrlHistoryService;

        public ImportProductVariantPriceBreakServiceTests()
        {
            _productVariantService = A.Fake<IProductVariantService>();
            _importProductUrlHistoryService = new ImportProductVariantPriceBreaksService(_productVariantService);
        }


        [Fact]
        public void ImportProductVariantPriceBreakService_ImportVariantPriceBreaks_ShouldCallProductVariantServiceAddPriceBreak()
        {
            var mockingKernel = new MockingKernel();
            var session = A.Fake<ISession>();
            mockingKernel.Bind<ISession>().ToMethod(context => session).InSingletonScope();
            MrCMSApplication.OverrideKernel(mockingKernel);

            var productVariantDTO = new ProductVariantImportDataTransferObject
            {
                PriceBreaks = new Dictionary<int, decimal>(){{10,299}}
            };
            var productVariant = new ProductVariant();

            var priceBreak = new PriceBreak() { Price = 299, Quantity = 10 };

            _importProductUrlHistoryService.ImportVariantPriceBreaks(productVariantDTO, productVariant);

            A.CallTo(() => _productVariantService.AddPriceBreak(priceBreak)).MustHaveHappened();
        }
    }
}