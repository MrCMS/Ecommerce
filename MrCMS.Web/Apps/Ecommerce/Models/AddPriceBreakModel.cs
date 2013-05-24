using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class AddPriceBreakModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        [Remote("IsQuantityValid", "PriceBreak", AdditionalFields = "Id,Type")]
        public int Quantity { get; set; }
        [Remote("IsPriceValid", "PriceBreak", AdditionalFields = "Id,Type,Quantity")]
        public decimal Price { get; set; }
    }
}