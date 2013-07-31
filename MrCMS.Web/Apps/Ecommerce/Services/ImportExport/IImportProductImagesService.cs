using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductImagesService
    {
        /// <summary>
        /// Add Product Images
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="product"></param>
        IEnumerable<MediaFile> ImportProductImages(List<string> images, MediaCategory mediaCategory);

        /// <summary>
        /// Add image to Product Gallery
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="mediaCategory"></param>
        bool ImportImageToGallery(string fileLocation, MediaCategory mediaCategory);
    }
}