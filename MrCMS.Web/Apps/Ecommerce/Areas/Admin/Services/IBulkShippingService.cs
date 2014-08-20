using System.Collections.Generic;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IBulkShippingService
    {
        Dictionary<string, List<string>> Update(Stream file);
    }
}