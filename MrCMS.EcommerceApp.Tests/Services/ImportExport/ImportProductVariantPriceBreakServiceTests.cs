using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Website;
using NHibernate;
using Ninject.MockingKernel;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ImportProductVariantPriceBreakServiceTests
    {
        private readonly IImportProductVariantPriceBreaksService _importProductUrlHistoryService;

        public ImportProductVariantPriceBreakServiceTests(ISession session)
        {
            _importProductUrlHistoryService = new ImportProductVariantPriceBreaksService(session);
        }

        [Fact(Skip = "To be refactored")]
        public void ImportProductVariantPriceBreakService_ImportVariantPriceBreaks_ShouldCallProductVariantServiceAddPriceBreak()
        {
            var mockingKernel = new MockingKernel();
            var session = A.Fake<ISession>();
            mockingKernel.Bind<ISession>().ToMethod(context => session).InSingletonScope();
            MrCMSKernel.OverrideKernel(mockingKernel);

            var productVariantDTO = new ProductVariantImportDataTransferObject
            {
                PriceBreaks = new Dictionary<int, decimal>() { { 10, 299 } }
            };
            var productVariant = new ProductVariant();

            var priceBreaks = _importProductUrlHistoryService.ImportVariantPriceBreaks(productVariantDTO, productVariant);

            priceBreaks.Should().HaveCount(1);
            var priceBreak = priceBreaks.ToList()[0];
            priceBreak.Price.Should().Be(299);
            priceBreak.Quantity.Should().Be(10);
            priceBreak.ProductVariant.Should().Be(productVariant);
        }
    }
}