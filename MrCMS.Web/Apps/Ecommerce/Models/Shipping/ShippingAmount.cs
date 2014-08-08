namespace MrCMS.Web.Apps.Ecommerce.Models.Shipping
{
    public class ShippingAmount
    {
        private ShippingAmount()
        {
        }

        public static readonly ShippingAmount NoneAvailable = new ShippingAmount();
        public static ShippingAmount Amount(decimal amount)
        {
            return new ShippingAmount { Value = amount };
        }
        public decimal Value { get; private set; }
    }
}