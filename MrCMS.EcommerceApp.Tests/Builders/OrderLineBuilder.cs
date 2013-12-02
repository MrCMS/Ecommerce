using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class OrderLineBuilder
    {
        private string _downloadFileUrl;
        private string _downloadContentType;
        private string _downloadFileName;
        private int _numberOfDownloads;

        public OrderLineBuilder()
        {
            _downloadFileUrl = "test-url";
            _downloadContentType = "content-type";
            _downloadFileName = "file-name";
        }

        public OrderLineBuilder WithFileUrl(string fileUrl)
        {
            _downloadFileUrl = fileUrl;
            return this;
        }

        public OrderLineBuilder WithContentType(string contentType)
        {
            _downloadContentType = contentType;
            return this;
        }

        public OrderLineBuilder WithFileName(string fileName)
        {
            _downloadFileName = fileName;
            return this;
        }
        public OrderLineBuilder WithNumberOfDownloads(int numberOfDownloads)
        {
            _numberOfDownloads = numberOfDownloads;
            return this;
        }

        public OrderLine Build()
        {
            return new OrderLine
                       {
                           DownloadFileUrl = _downloadFileUrl,
                           DownloadFileContentType = _downloadContentType,
                           DownloadFileName = _downloadFileName,
                           NumberOfDownloads = _numberOfDownloads
                       };
        }
    }
}