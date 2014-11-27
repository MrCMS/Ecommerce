namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public interface IPerformNopImport
    {
        ImportResult Execute(INopCommerceProductReader nopCommerceProductReader, string connectionString);
    }
}