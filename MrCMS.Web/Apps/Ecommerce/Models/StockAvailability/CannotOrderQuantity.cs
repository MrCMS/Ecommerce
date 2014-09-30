using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Models.StockAvailability
{
    public class CannotOrderQuantity : CanBuyStatus
    {
        private readonly int _requestedQuantity;
        private readonly ProductVariant _variant;

        public CannotOrderQuantity(ProductVariant variant, int requestedQuantity)
        {
            _variant = variant;
            _requestedQuantity = requestedQuantity;
        }

        public override bool OK
        {
            get { return false; }
        }

        public override string Message
        {
            get
            {
                return string.Format("Sorry, but there are currently only {0} units of {1} in stock",
                    _variant.StockRemaining, _variant.DisplayName);
            }
        }
    }
}