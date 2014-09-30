using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Models.StockAvailability
{
    public class OutOfStock : CanBuyStatus
    {
        private readonly ProductVariant _variant;

        public OutOfStock(ProductVariant variant)
        {
            _variant = variant;
        }

        public override bool OK
        {
            get { return false; }
        }

        public override string Message
        {
            get { return string.Format("Sorry, but {0} is currently out of stock", _variant.DisplayName); }
        }
    }
}