using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.TestableModels
{
    public class TestableCartModel : CartModel
    {
        private readonly decimal? _totalPreShipping;
        private readonly decimal? _shippableCalculationTotal;
        private readonly decimal? _weight;

        public TestableCartModel(decimal? weight = null, decimal? totalPreShipping = null, decimal? shippableCalculationTotal=null)
        {
            _weight = weight;
            _totalPreShipping = totalPreShipping;
            _shippableCalculationTotal = shippableCalculationTotal;
        }

        public override decimal Weight
        {
            get { return _weight ?? base.Weight; }
        }

        public override decimal TotalPreShipping
        {
            get { return _totalPreShipping ?? base.TotalPreShipping; }
        }
        public override decimal ShippableCalculationTotal
        {
            get { return _shippableCalculationTotal ?? base.ShippableCalculationTotal; }
        }
    }
}