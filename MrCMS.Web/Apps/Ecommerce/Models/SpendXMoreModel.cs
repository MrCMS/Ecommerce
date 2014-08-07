using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class SpendXMoreModel
    {
        public virtual decimal SpendAmountMore { get; set; }

        public virtual bool Show
        {
            get { return SpendAmountMore > 0; }
        }

        public virtual string DisplayMessage
        {
            get
            {
                return SpendAmountMore > 0
                    ? string.Format("Spend {0} more to get free Shipping!", SpendAmountMore.ToCurrencyFormat())
                    : string.Empty;
            }
        }
    }
}