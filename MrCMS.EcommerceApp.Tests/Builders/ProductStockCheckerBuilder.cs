using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ProductStockCheckerBuilder
    {
        private EcommerceSettings _ecommerceSettings = new EcommerceSettings();
        private ISession _session = A.Fake<ISession>();


        public ProductStockChecker Build()
        {
            return new ProductStockChecker(_session, _ecommerceSettings);
        }

        public ProductStockCheckerBuilder WithSession(ISession session)
        {
            _session = session;
            return this;
        }


        public ProductStockCheckerBuilder WithWarehousesEnabled()
        {
            _ecommerceSettings.WarehouseStockEnabled = true;
            return this;
        }
    }
}