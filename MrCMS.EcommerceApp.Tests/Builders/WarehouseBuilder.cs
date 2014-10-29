using MrCMS.Web.Apps.Ecommerce.Stock.Entities;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class WarehouseBuilder : IEntityBuilder<Warehouse>
    {
        private string _name = "Warehouse";

        public Warehouse Build()
        {
            return new Warehouse
            {
                Name = _name
            };
        }

        public WarehouseBuilder WithName(string name)
        {
            _name = name;
            return this;
        }
    }
}