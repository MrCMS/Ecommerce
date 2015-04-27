using FakeItEasy;
using MrCMS.Batching.Services;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using NHibernate;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport.ImportProductsServiceTests
{
    public class ImportProductsServiceBuilder
    {
        private readonly ICreateBatch _createBatch = A.Fake<ICreateBatch>();
        private readonly IDocumentService _documentService = A.Fake<IDocumentService>();
        private readonly IGetNewBrandPage _getNewBrandPage = A.Fake<IGetNewBrandPage>();
        private readonly IImportProductImagesService _importProductImagesService = A.Fake<IImportProductImagesService>();

        private readonly IImportProductSpecificationsService _importProductSpecificationsService =
            A.Fake<IImportProductSpecificationsService>();

        private readonly IImportProductVariantsService _importProductVariantsService =
            A.Fake<IImportProductVariantsService>();

        private readonly IImportProductUrlHistoryService _importUrlHistoryService =
            A.Fake<IImportProductUrlHistoryService>();

        private readonly ISession _session;

        private IUniquePageService _uniquePageService;

        public ImportProductsServiceBuilder(ISession session)
        {
            _session = session;
        }

        public ImportProductsService Build()
        {
            _uniquePageService = A.Fake<IUniquePageService>();
            var importProductsService = new ImportProductsService(_documentService,
                _importProductSpecificationsService,
                _importProductVariantsService,
                _importProductImagesService, _importUrlHistoryService,
                _session, _uniquePageService, _createBatch, _getNewBrandPage);
            return importProductsService;
        }
    }
}