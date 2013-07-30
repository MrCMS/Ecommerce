using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductImagesService : IImportProductImagesService
    {
        private readonly IFileService _fileService;

        public ImportProductImagesService(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Add Product Images
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="product"></param>
        public IEnumerable<MediaFile> ImportProductImages(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            // We want to always look at all of the urls in the file, and only not import when it is set to update = no
            foreach (var imageUrl in dataTransferObject.Images)
            {
                Uri result;
                if (Uri.TryCreate(imageUrl, UriKind.Absolute, out result))
                {
                    // substring(1) should remove the leading ? 
                    if (!String.IsNullOrWhiteSpace(result.Query))
                    {
                        if (result.Query.Contains("update=no"))
                        {
                            continue;
                        }
                    }
                    var resultWithOutQuery= !string.IsNullOrEmpty(result.Query) ? result.ToString().Replace(result.Query, "") : result.ToString();
                    ImportImageToGallery(resultWithOutQuery, product.Gallery);
                }
            }

            return dataTransferObject.Images.Any() ? product.Images : null;
        }

        /// <summary>
        /// Add image to Product Gallery
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="mediaCategory"></param>
        public bool ImportImageToGallery(string fileLocation, MediaCategory mediaCategory)
        {
            // rather than using webclient, this has been refactored to use HttpWebRequest/Response 
            // so that we can get the content type from the response, rather than assuming it
            try
            {
                var httpWebRequest = HttpWebRequest.Create(fileLocation) as HttpWebRequest;

                var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                using (var responseStream = httpWebResponse.GetResponseStream())
                {
                    var memoryStream = new MemoryStream();
                    responseStream.CopyTo(memoryStream);

                    var fileName = Path.GetFileName(fileLocation);
                    _fileService.AddFile(memoryStream, fileName, httpWebResponse.ContentType,
                                         (int)memoryStream.Length, mediaCategory);
                }
                return true;
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return false;
            }
        }
    }
}