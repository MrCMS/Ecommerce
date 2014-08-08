namespace MrCMS.Web.Apps.Ecommerce.Models.Shipping
{
    public class ShippingAmount
    {
        private ShippingAmount()
        {
        }

        public static readonly ShippingAmount NoneAvailable = new ShippingAmount();
        public static ShippingAmount Total(decimal amount)
        {
            return new ShippingAmount { Amount = amount };
        }
        public decimal Amount { get; private set; }
    }
}