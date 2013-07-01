using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class AddPriceBreakModel
    {
        public int Id { get; set; }
        [Remote("IsQuantityValid", "PriceBreak", AdditionalFields = "Id")]
        public int Quantity { get; set; }
        [Remote("IsPriceValid", "PriceBreak", AdditionalFields = "Id,Quantity")]
        public decimal Price { get; set; }
    }
}