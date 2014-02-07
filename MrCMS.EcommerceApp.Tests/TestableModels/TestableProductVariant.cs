using MrCMS.DbConfiguration.Mapping;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.EcommerceApp.Tests.TestableModels
{
    [DoNotMap]
    public class TestableProductVariant : ProductVariant
    {
        private readonly bool? _inStockStatus;

        public TestableProductVariant(bool? inStockStatus)
        {
            _inStockStatus = inStockStatus;
        }

        public override bool InStock
        {
            get
            {
                return _inStockStatus ?? base.InStock;
            }
        }
    }
}