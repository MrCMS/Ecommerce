using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
{
    public interface ITaxRateManager
    {
        TaxRate Get(int id);
        TaxRate GetDefaultRate();
        TaxRate GetRateForOrderLine(OrderLine orderLine);
        IList<TaxRate> GetAll();
        void Add(TaxRate taxRate);
        void Update(TaxRate taxRate);
        void Delete(TaxRate taxRate);
        void SetAllDefaultToFalse();
    }
}