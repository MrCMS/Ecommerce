namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs
{
    public class BulkShippingUpdateDataTransferObject 
    {
        public int OrderId { get; set; }
        public string ShippingMethod { get; set; }
        public string TrackingNumber { get; set; }
    }
}