namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public interface IBuyableItem
    {
        int Id { get; }
        string SKU { get; }
        decimal TaxRatePercentage { get; }
        decimal GetPrice(int quantity);
        decimal GetSaving(int quantity);
        decimal GetTax(int quantity);
        decimal GetUnitPrice();
        bool CanBuy(int quantity);
        decimal GetPricePreTax(int quantity);
        decimal Weight { get; }
        string Name { get; }
        string EditUrl { get; }
    }
}