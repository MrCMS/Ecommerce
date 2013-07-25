using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using Xunit;
using MrCMS.Services;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport
{
    public class ImportProductUrlHistoryServiceTests
    {
        private readonly IUrlHistoryService _urlHistoryService;
        private readonly IImportProductUrlHistoryService _importProductUrlHistoryService;

        public ImportProductUrlHistoryServiceTests()
        {
            _urlHistoryService = A.Fake<IUrlHistoryService>();
            _importProductUrlHistoryService = new ImportProductUrlHistoryService(_urlHistoryService);
        }


        [Fact(Skip = "To be refactored")]
        public void ImportProductUrlHistoryService_ImportUrlHistory_ShouldCallUrlHistoryServiceAdd()
        {
            var productDTO = new ProductImportDataTransferObject
                                 {
                                     UrlSegment = "test-url",
                                     UrlHistory = new List<string>(){"test-url"}
                                 };

            var product = new Product() { UrlSegment = "test-url" };

            var urlHistory = new UrlHistory() { UrlSegment = "test-url", Webpage = product };
            A.CallTo(() => _urlHistoryService.GetByUrlSegment("test-url")).Returns(null);

            _importProductUrlHistoryService.ImportUrlHistory(productDTO, product);

            A.CallTo(() => _urlHistoryService.Add(urlHistory)).MustHaveHappened();
        }
    }
}