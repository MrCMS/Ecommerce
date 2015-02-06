namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public interface IPerformNopImport
    {
        ImportResult Execute(NopCommerceDataReader dataReader);

        ImportResult UpdateOrdersAndUsers(NopCommerceDataReader dataReader);
    }
}