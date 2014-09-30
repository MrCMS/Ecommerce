using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Models.StockAvailability
{
    public class NoShippingMethodWouldBeAvailable : CanBuyStatus
    {
        private readonly ProductVariant _variant;

        public NoShippingMethodWouldBeAvailable(ProductVariant variant)
        {
            _variant = variant;
        }

        public override bool OK
        {
            get { return false; }
        }

        public override string Message
        {
            get
            {
                return
                    string.Format(
                        "You cannot order {0} as adding it to your cart would mean that there are no availble shipping methods",
                        _variant.DisplayName);
            }
        }
    }
}