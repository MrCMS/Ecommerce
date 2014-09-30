namespace MrCMS.Web.Apps.Ecommerce.Models.StockAvailability
{
    public abstract class CanBuyStatus
    {
        public abstract bool OK { get; }
        public abstract string Message { get; }
    }
}