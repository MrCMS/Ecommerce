using System.Collections.Generic;
using System.Linq;
using Elmah.ContentSyndication;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public class PerformSimpleBulkUpdate : IPerformSimpleBulkUpdate
    {
        private readonly IStatelessSession _statelessSession;
        private readonly Site _site;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly HashSet<ProductVariant> _variantsToUpdate;

        public PerformSimpleBulkUpdate(IStatelessSession statelessSession, Site site, IStringResourceProvider stringResourceProvider)
        {
            _statelessSession = statelessSession;
            _site = site;
            _stringResourceProvider = stringResourceProvider;
            _variantsToUpdate = new HashSet<ProductVariant>();
        }

        public BulkUpdateResult Update(IEnumerable<BulkStockUpdateDataTransferObject> items)
        {
            var allVariants =
                _statelessSession.QueryOver<ProductVariant>()
                    .Where(variant => variant.Site.Id == _site.Id && !variant.IsDeleted && variant.SKU != null)
                    .List().ToDictionary(variant => variant.SKU, variant => variant);


            var bulkStockUpdateDataTransferObjects = items.ToHashSet();
            using (var transaction = _statelessSession.BeginTransaction())
            {
                foreach (var dto in bulkStockUpdateDataTransferObjects)
                {
                    ProductVariant variant = allVariants.ContainsKey(dto.SKU) ? allVariants[dto.SKU] : null;

                    if (variant != null && variant.StockRemaining != dto.StockRemaining)
                    {
                        variant.StockRemaining = dto.StockRemaining;
                        _variantsToUpdate.Add(variant);
                    }
                }
                _variantsToUpdate.ForEach(_statelessSession.Update);
                transaction.Commit();
            }

            return new BulkUpdateResult
            {
                IsSuccess = true,
                Messages = new List<string>
                {
                    string.Format(
                        _stringResourceProvider.GetValue("Bulk Stock Update - number processed", "{0} items processed"),
                        bulkStockUpdateDataTransferObjects.Count()),
                    string.Format(
                        _stringResourceProvider.GetValue("Bulk Stock Update - number updated", "{0} items updated"),
                        _variantsToUpdate.Count)
                }
            };
        }
    }
}