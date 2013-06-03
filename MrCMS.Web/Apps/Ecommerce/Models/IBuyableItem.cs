namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public interface IBuyableItem
    {
        int Id { get; }
        string SKU { get; }
        decimal TaxRatePercentage { get; }
        decimal Price { get; }
        decimal GetPrice(int quantity);
        decimal ReducedBy { get; }
        decimal Tax { get; }
        bool CanBuy(int quantity);
        decimal PricePreTax { get; }
        decimal Weight { get; }
        string Name { get; }
        string EditUrl { get; }
    }
}