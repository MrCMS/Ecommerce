namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IUpdateStockAdminService
    {
        void UpdateVariantStock(int id, int stockRemaining);
    }
}