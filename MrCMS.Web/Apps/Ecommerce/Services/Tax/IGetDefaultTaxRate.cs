using MrCMS.Web.Apps.Ecommerce.Entities.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
{
    public interface IGetDefaultTaxRate
    {
        TaxRate Get();
    }
}