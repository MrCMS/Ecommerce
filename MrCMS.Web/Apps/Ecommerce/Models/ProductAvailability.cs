namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public enum ProductAvailability
    {
        PreOrder,
        Available,
        BackOrder,
        EndOfLine,
        Unavailable
    }

    public interface ICanAddToCart
    {
        int Id { get; }
        string SKU { get; }
        decimal TaxRatePercentage { get; }
        decimal Price { get; }
        decimal ReducedBy { get; }
        decimal Tax { get; }
        bool CanBuy(int quantity);
        decimal PricePreTax { get; }
        decimal Weight { get; }
        string Name { get; }
    }
}