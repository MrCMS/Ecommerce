using System.Drawing;
using MrCMS.Services;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services
{
    public class NewsletterBuilderHelperService : INewsletterBuilderHelperService
    {
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileService _fileService;

        public NewsletterBuilderHelperService(IImageProcessor imageProcessor, IFileService fileService)
        {
            _imageProcessor = imageProcessor;
            _fileService = fileService;
        }

        public string GetResizedImage(string imageUrl, Size size)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var mediaFile = _imageProcessor.GetImage(imageUrl);
                if (mediaFile != null)
                    return _fileService.GetFileLocation(mediaFile, size);
            }

            return string.Empty;
        }
    }
}