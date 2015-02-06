using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class DiscountInfo
    {
        private readonly CheckLimitationsResult _checkLimitationsResult;
        private readonly Discount _discount;
        private bool _applied;

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
                return
                    CheckLimitationsResult.Status == CheckLimitationsResultStatus.NeverValid
                        ? DiscountStatus.NeverValid
                        : CheckLimitationsResult.Success
                            ? _applied
                                ? DiscountStatus.Applied
                                : DiscountStatus.ValidButNotApplied
                            : Discount.RequiresCode
                                ? DiscountStatus.ExplicitAndInvalid
                                : DiscountStatus.AutomaticAndInvalid;
            }
        }

        public void MarkAsApplied()
        {
            _applied = true;
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