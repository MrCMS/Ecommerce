using System.Drawing.Printing;
using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class GenerateWarehouseStockOnAdd : IOnAdded<ProductVariant>
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IGenerateProductVariantWarehouseStock _generateProductVariantWarehouseStock;

        public GenerateWarehouseStockOnAdd(EcommerceSettings ecommerceSettings, IGenerateProductVariantWarehouseStock generateProductVariantWarehouseStock)
        {
            _ecommerceSettings = ecommerceSettings;
            _generateProductVariantWarehouseStock = generateProductVariantWarehouseStock;
        }

        public void Execute(OnAddedArgs<ProductVariant> args)
        {
            if (!_ecommerceSettings.WarehouseStockEnabled)
                return;
            var productVariant = args.Item;
            if (productVariant.TrackingPolicy != TrackingPolicy.Track)
                return;
            _generateProductVariantWarehouseStock.Generate(productVariant);
        }
    }
}