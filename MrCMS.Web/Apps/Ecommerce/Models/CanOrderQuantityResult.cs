namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CanOrderQuantityResult
    {
        public bool CanOrder { get; set; }
        public int StockRemaining{ get; set; }
    }
}