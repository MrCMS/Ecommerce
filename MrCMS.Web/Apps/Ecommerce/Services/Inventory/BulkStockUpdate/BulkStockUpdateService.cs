using System.Collections.Generic;
using System.Linq;
using Elmah.ContentSyndication;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public class BulkStockUpdateService : IBulkStockUpdateService
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly Site _site;
        private readonly HashSet<ProductVariant> _variantsToUpdate;
        private Dictionary<string, ProductVariant> _allVariants;

        public BulkStockUpdateService(ISessionFactory sessionFactory, Site site)
        {
            _sessionFactory = sessionFactory;
            _site = site;
            _variantsToUpdate = new HashSet<ProductVariant>();
        }

        public int BulkStockUpdateFromDTOs(IEnumerable<BulkStockUpdateDataTransferObject> items)
        {
            using (IStatelessSession statelessSession = _sessionFactory.OpenStatelessSession())
            {
                _allVariants =
                    statelessSession.QueryOver<ProductVariant>()
                        .Where(variant => variant.Site.Id == _site.Id && !variant.IsDeleted && variant.SKU != null)
                        .List().ToDictionary(variant => variant.SKU, variant => variant);

                int noOfUpdatedItems = 0;

                using (var transaction = statelessSession.BeginTransaction())
                {

                    foreach (BulkStockUpdateDataTransferObject dataTransferObject in items)
                    {
                        BulkStockUpdate(dataTransferObject, ref noOfUpdatedItems);
                    }
                    _variantsToUpdate.ForEach(statelessSession.Update);
                    transaction.Commit();
                }

                return noOfUpdatedItems;
            }
        }

        public void BulkStockUpdate(BulkStockUpdateDataTransferObject itemDto, ref int noOfUpdatedItems)
        {
            if (_allVariants == null)
                _allVariants = new Dictionary<string, ProductVariant>();

            ProductVariant variant = _allVariants.ContainsKey(itemDto.SKU) ? _allVariants[itemDto.SKU] : null;

            if (variant != null && variant.StockRemaining != itemDto.StockRemaining)
            {
                variant.StockRemaining = itemDto.StockRemaining;
                _variantsToUpdate.Add(variant);
                noOfUpdatedItems++;
            }
        }
    }
}