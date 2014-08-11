namespace MrCMS.Web.Apps.Ecommerce.Services.Pricing
{
    public interface IProductPricingService
    {
        decimal GetPriceIncludingTax(decimal? amount, decimal taxRate);
        decimal GetPriceExcludingTax(decimal? amount, decimal taxRate);
        decimal GetTax(decimal? amount, decimal taxRate);
    }
}