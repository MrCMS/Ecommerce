using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IExportOrdersService
    {
        byte[] ExportOrderToPdf(Order order);
    }
}