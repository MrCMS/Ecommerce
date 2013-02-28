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
        string SKU { get; }
        decimal TaxRatePercentage { get; }
        decimal Price { get; }
        decimal ReducedBy { get; }
        decimal Tax { get; }
        bool CanBuy(int quantity);
        decimal PricePreTax { get; }
    }
}