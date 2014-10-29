namespace MrCMS.Web.Apps.Ecommerce.Models.StockAvailability
{
    public class CanBuy : CanBuyStatus
    {
        public override bool OK
        {
            get { return true; }
        }

        public override string Message
        {
            get { return null; }
        }
    }
}