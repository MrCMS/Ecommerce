namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class LowStockReportSearchModel
    {
        public LowStockReportSearchModel()
        {
            Page = 1;
            Threshold = 10;
        }

        public int Threshold { get; set; }

        public int Page { get; set; }
    }
}