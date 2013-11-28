using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download
{
    public class DownloadValidationResult
    {
        private readonly IEnumerable<string> _errors;

        public DownloadValidationResult(IEnumerable<string> errors)
        {
            _errors = errors;
        }

        public bool Success { get { return !Errors.Any(); } }

        public IEnumerable<string> Errors
        {
            get { return _errors; }
        }
    }
}