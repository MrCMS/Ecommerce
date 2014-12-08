using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class DiscountInfo
    {
        private readonly Discount _discount;
        private readonly CheckLimitationsResult _checkLimitationsResult;

        public DiscountInfo(Discount discount, CheckLimitationsResult checkLimitationsResult)
        {
            _discount = discount;
            _checkLimitationsResult = checkLimitationsResult;
        }

        public Discount Discount
        {
            get { return _discount; }
        }

        public DiscountStatus Status
        {
            get
            {
                return CheckLimitationsResult.Success
                    ? DiscountStatus.ToApply
                    : Discount.RequiresCode
                        ? DiscountStatus.ExplicitAndInvalid
                        : DiscountStatus.AutomaticAndInvalid;
            }
        }

        public string[] Messages
        {
            get { return CheckLimitationsResult.Messages; }
        }

        public CheckLimitationsResult CheckLimitationsResult
        {
            get { return _checkLimitationsResult; }
        }
    }
}