using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class PriceBreak : SiteEntity
    {
        public virtual ProductVariant Item { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal Price { get; set; }
        public virtual decimal PriceExcludingTax
        {
            get { return TaxAwareProductPrice.GetPriceExcludingTax(Price, TaxRate); }
        }
        public virtual decimal PriceIncludingTax
        {
            get { return TaxAwareProductPrice.GetPriceIncludingTax(Price, TaxRate); }
        }

        protected virtual TaxRate TaxRate
        {
            get
            {
                return Item != null ? Item.TaxRate : null;
            }
        }
    }
}