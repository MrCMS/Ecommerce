using MrCMS.EcommerceApp.Tests.Models.CartModelTests;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class GiftCardBuilder
    {
        private decimal? _availableAmount;

        public GiftCardBuilder WithAvailableAmount(decimal availableAmount)
        {
            _availableAmount = availableAmount;
            return this;
        }

        public GiftCard Build()
        {
            return new TestableGiftCard(_availableAmount)
            {

            };
        }
    }
}