using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
{
    public interface ITaxRateManager
    {
        TaxRate Get(int id);
        IList<TaxRate> GetAll();
        void Add(TaxRate taxRate);
        void Update(TaxRate taxRate);
        void Delete(TaxRate taxRate);
        List<SelectListItem> GetOptions(TaxRate taxRate = null);
        void SetAllDefaultToFalse();
    }
}