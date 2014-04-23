using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Entities.Documents;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport.ImportProductsServiceTests
{
    public class ImportProductsServiceBuilder
    {
        private IDocumentService _documentService = A.Fake<IDocumentService>();
        private IBrandService _brandService = A.Fake<IBrandService>();
        private IImportProductSpecificationsService _importProductSpecificationsService = A.Fake<IImportProductSpecificationsService>();
        private IImportProductVariantsService _importProductVariantsService = A.Fake<IImportProductVariantsService>();
        private IImportProductImagesService _importProductImagesService = A.Fake<IImportProductImagesService>();
        private IImportProductUrlHistoryService _importUrlHistoryService = A.Fake<IImportProductUrlHistoryService>();
        private ISession _session = A.Fake<ISession>();
        private IEnumerable<Document> _documents = new List<Document>();
        private IUniquePageService _uniquePageService;

        public ImportProductsService Build()
        {
            _uniquePageService = A.Fake<IUniquePageService>();
            var importProductsService = new ImportProductsService(_documentService, _brandService,
                                                                  _importProductSpecificationsService,
                                                                  _importProductVariantsService,
                                                                  _importProductImagesService, _importUrlHistoryService,
                                                                  _session,_uniquePageService);
            importProductsService.SetAllDocuments(_documents);
            return importProductsService;
        }

        public ImportProductsServiceBuilder WithExistingDocuments(params Document[] documents)
        {
            _documents = documents;
            return this;
        }
    }
}