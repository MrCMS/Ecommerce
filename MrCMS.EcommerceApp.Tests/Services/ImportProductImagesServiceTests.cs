using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class ImportProductImagesServiceTests
    {
        private readonly IFileService _fileService;
        private readonly ImportProductImagesService _importProductImagesService;

        public ImportProductImagesServiceTests()
        {
            _fileService = A.Fake<IFileService>();
            _importProductImagesService = new ImportProductImagesService(_fileService);
        }

        [Fact]
        public void ImportProductImagesService_ImportImageToGallery_ShouldCallReturnTrue()
        {
            var result =
                _importProductImagesService.ImportImageToGallery("http://www.thought.co.uk/Content/images/logo-white.png",
                                                                 null);

            result.Should().BeTrue();
        }
    }
}