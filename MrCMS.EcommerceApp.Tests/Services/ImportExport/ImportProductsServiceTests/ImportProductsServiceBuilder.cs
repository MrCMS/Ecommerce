using FakeItEasy;
using MrCMS.Batching.Services;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport.ImportProductsServiceTests
{
    public class ImportProductsServiceBuilder
    {
        private readonly ICreateBatch _createBatch = A.Fake<ICreateBatch>();
        private readonly IGetNewBrandPage _getNewBrandPage = A.Fake<IGetNewBrandPage>();

        private readonly IImportProductImagesService _importProductImagesService = A.Fake<IImportProductImagesService>()
            ;

        private readonly IImportProductSpecificationsService _importProductSpecificationsService =
            A.Fake<IImportProductSpecificationsService>();

        private readonly IImportProductVariantsService _importProductVariantsService =
            A.Fake<IImportProductVariantsService>();

        private readonly IImportProductUrlHistoryService _importUrlHistoryService =
            A.Fake<IImportProductUrlHistoryService>();

        private readonly ISession _session;
        private readonly IGetDocumentByUrl<MediaCategory> _getByUrl = A.Fake<IGetDocumentByUrl<MediaCategory>>();
        private readonly IMediaCategoryAdminService _mediaCategoryAdminService = A.Fake<IMediaCategoryAdminService>();

        private IUniquePageService _uniquePageService;

        public ImportProductsServiceBuilder(ISession session)
        {
            _session = session;
        }

        public ImportProductsService Build()
        {
            _uniquePageService = A.Fake<IUniquePageService>();
            var importProductsService = new ImportProductsService(
                _getByUrl, _mediaCategoryAdminService,
                _importProductSpecificationsService,
                _importProductVariantsService,
                _importProductImagesService, _importUrlHistoryService,
                _session, _uniquePageService, _createBatch, _getNewBrandPage);
            return importProductsService;
        }
    }
}