using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
{
    public interface IGetTaxRateOptions
    {
        List<SelectListItem> GetOptions(TaxRate rate);
        List<SelectListItem> GetOptions(int? id = null);
    }
}