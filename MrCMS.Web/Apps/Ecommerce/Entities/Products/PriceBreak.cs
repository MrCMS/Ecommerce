using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Helpers.Pricing;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class PriceBreak : SiteEntity
    {
        public virtual ProductVariant ProductVariant { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal Price { get; set; }

        //public virtual decimal PriceExcludingTax
        //{
        //    get { return Price.ProductPriceExcludingTax(TaxRatePercentage); }
        //}

        //public virtual decimal PriceIncludingTax
        //{
        //    get { return Price.ProductPriceIncludingTax(TaxRatePercentage); }
        //}

        //public virtual decimal Tax
        //{
        //    get { return Price.ProductTax(TaxRatePercentage); }
        //}

        //protected virtual decimal TaxRatePercentage
        //{
        //    get { return ProductVariant != null ? ProductVariant.TaxRatePercentage : decimal.Zero; }
        //}
    }
}