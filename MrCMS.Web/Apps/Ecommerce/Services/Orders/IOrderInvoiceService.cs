using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderInvoiceService
    {
        byte[] GeneratePDF(Order order);
    }
}