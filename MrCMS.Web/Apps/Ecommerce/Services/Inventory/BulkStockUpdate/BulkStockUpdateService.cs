using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public class BulkStockUpdateService : IBulkStockUpdateService
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ISession _session;
        private IList<ProductVariant> _allVariants;
        private readonly List<ProductVariant> _variantsToUpdate;

        public BulkStockUpdateService(IProductVariantService productVariantService, ISession session)
        {
            _productVariantService = productVariantService;
            _session = session;
            _allVariants = new List<ProductVariant>();
            _variantsToUpdate = new List<ProductVariant>();
        }

        public int BulkStockUpdateFromDTOs(IEnumerable<BulkStockUpdateDataTransferObject> items)
        {
            _allVariants = _productVariantService.GetAll();

            var noOfUpdatedItems = 0;
            
            _session.Transact(session =>
               {
                   foreach (var dataTransferObject in items)
                   {
                       BulkStockUpdate(dataTransferObject, ref noOfUpdatedItems);
                   }
                   _variantsToUpdate.ForEach(session.SaveOrUpdate);
               });

            return noOfUpdatedItems;
        }

        public void BulkStockUpdate(BulkStockUpdateDataTransferObject itemDto, ref int noOfUpdatedItems)
        {
            if (_allVariants == null)
                _allVariants = new List<ProductVariant>();

            var item = _allVariants.SingleOrDefault(x => x.SKU == itemDto.SKU);

            if (item != null && item.StockRemaining != itemDto.StockRemaining)
            {
                item.StockRemaining = itemDto.StockRemaining;
                _variantsToUpdate.Add(item);
                noOfUpdatedItems++;
            }
        }
    }
}