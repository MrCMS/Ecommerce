using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class RemoveWarehouseIfSwitchedToDontTrack : IOnUpdated<ProductVariant>
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IRemoveProductVariantWarehouseStock _generateProductVariantWarehouseStock;

        public RemoveWarehouseIfSwitchedToDontTrack(EcommerceSettings ecommerceSettings, IRemoveProductVariantWarehouseStock generateProductVariantWarehouseStock)
        {
            _ecommerceSettings = ecommerceSettings;
            _generateProductVariantWarehouseStock = generateProductVariantWarehouseStock;
        }

        public void Execute(OnUpdatedArgs<ProductVariant> args)
        {
            if (!_ecommerceSettings.WarehouseStockEnabled)
                return;

            var current = args.Item;
            var previous = args.Original;

            if (current.TrackingPolicy == TrackingPolicy.DontTrack && previous.TrackingPolicy == TrackingPolicy.Track)
                _generateProductVariantWarehouseStock.Remove(current);
        }
    }
}