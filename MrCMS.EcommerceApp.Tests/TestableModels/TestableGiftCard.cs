using MrCMS.DbConfiguration.Mapping;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;

namespace MrCMS.EcommerceApp.Tests.TestableModels
{
    [DoNotMap]
    public class TestableGiftCard : GiftCard
    {
        private readonly decimal? _availableAmount;

        public TestableGiftCard(decimal? availableAmount)
        {
            _availableAmount = availableAmount;
        }

        public override decimal AvailableAmount
        {
            get { return _availableAmount ?? base.AvailableAmount; }
        }
    }
}