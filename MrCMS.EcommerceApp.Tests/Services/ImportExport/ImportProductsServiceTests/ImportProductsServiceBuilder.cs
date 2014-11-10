using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Batching.Services;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport.ImportProductsServiceTests
{
    public class ImportProductsServiceBuilder
    {
        private readonly ISession _session;

        public ImportProductsServiceBuilder(ISession session)
        {
            _session = session;
        }

        private IDocumentService _documentService = A.Fake<IDocumentService>();
        private IImportProductSpecificationsService _importProductSpecificationsService = A.Fake<IImportProductSpecificationsService>();
        private IImportProductVariantsService _importProductVariantsService = A.Fake<IImportProductVariantsService>();
        private IImportProductImagesService _importProductImagesService = A.Fake<IImportProductImagesService>();
        private IImportProductUrlHistoryService _importUrlHistoryService = A.Fake<IImportProductUrlHistoryService>();
        private IUniquePageService _uniquePageService;
        private ICreateBatchRun _createBatchRun = A.Fake<ICreateBatchRun>();

        public ImportProductsService Build()
        {
            _uniquePageService = A.Fake<IUniquePageService>();
            var importProductsService = new ImportProductsService(_documentService,
                                                                  _importProductSpecificationsService,
                                                                  _importProductVariantsService,
                                                                  _importProductImagesService, _importUrlHistoryService,
                                                                  _session, _uniquePageService, _createBatchRun);
            return importProductsService;
        }
    }
}