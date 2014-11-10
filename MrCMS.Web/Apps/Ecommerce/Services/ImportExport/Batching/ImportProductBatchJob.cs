using MrCMS.Batching.Entities;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Batching
{
    public class ImportProductBatchJob : BatchJob
    {
        public override string Name
        {
            get { return string.Format("Import Product Batch Job - {0} ({1})", ProductName, UrlSegment); }
        }

        public virtual string ProductName { get; set; }
        public virtual string UrlSegment { get; set; }

        public virtual ProductImportDataTransferObject DTO
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<ProductImportDataTransferObject>(Data);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}