using System.IO;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportPictureData : IImportPictureData
    {
        private const string NopProductImages = "nop-product-images";
        private readonly IFileService _fileService;
        private readonly IGetDocumentByUrl<MediaCategory> _getByUrl;
        private readonly IMediaCategoryAdminService _mediaCategoryAdminService;

        public ImportPictureData(IFileService fileService, IGetDocumentByUrl<MediaCategory> getByUrl, IMediaCategoryAdminService mediaCategoryAdminService)
        {
            _fileService = fileService;
            _getByUrl = getByUrl;
            _mediaCategoryAdminService = mediaCategoryAdminService;
        }

        public string ImportPictures(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var pictureData = dataReader.GetPictureData();

            var mediaCategory = _getByUrl.GetByUrl(NopProductImages);
            if (mediaCategory == null)
            {
                mediaCategory = new MediaCategory
                {
                    Name = "Nop Product Images",
                    UrlSegment = NopProductImages,
                    IsGallery = false,
                    HideInAdminNav = false
                };
                _mediaCategoryAdminService.Add(mediaCategory);
            }

            foreach (var data in pictureData)
            {
                using (var fileData = data.GetData())
                {
                    var memoryStream = new MemoryStream();
                    fileData.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    var mediaFile = _fileService.AddFile(memoryStream, data.FileName, data.ContentType, memoryStream.Length,
                        mediaCategory);
                    nopImportContext.AddEntry(data.Id, mediaFile);
                }
            }

            return string.Format("{0} pictures imported", pictureData.Count);
        }
    }
}