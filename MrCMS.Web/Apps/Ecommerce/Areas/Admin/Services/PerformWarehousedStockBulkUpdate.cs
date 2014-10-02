using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class PerformWarehousedStockBulkUpdate : IPerformWarehousedStockBulkUpdate
    {
        private readonly Site _site;
        private readonly IStatelessSession _statelessSession;
        private readonly IStringResourceProvider _stringResourceProvider;

        public PerformWarehousedStockBulkUpdate(IStatelessSession statelessSession, Site site,
            IStringResourceProvider stringResourceProvider)
        {
            _statelessSession = statelessSession;
            _site = site;
            _stringResourceProvider = stringResourceProvider;
        }

        public BulkUpdateResult Update(List<BulkWarehouseStockUpdateDTO> dtoList)
        {
            var warehouseStocks = _statelessSession.Query<WarehouseStock>()
                .Where(stock => stock.Site.Id == _site.Id && !stock.IsDeleted
                                && stock.ProductVariant != null
                                && stock.ProductVariant.SKU != null
                                && stock.Warehouse != null)
                .Fetch(stock => stock.ProductVariant)
                .Fetch(stock => stock.Warehouse)
                .ToList();
            var dictionary = warehouseStocks
                .GroupBy(stock => stock.ProductVariant.SKU)
                .ToDictionary(stocks => stocks.Key,
                    stocks => stocks.ToDictionary(stock => stock.Warehouse.Id, stock => stock));

            HashSet<BulkWarehouseStockUpdateDTO> dtos = dtoList.ToHashSet();
            var stockToUpdate = new HashSet<WarehouseStock>();
            using (ITransaction transaction = _statelessSession.BeginTransaction())
            {
                foreach (BulkWarehouseStockUpdateDTO dto in dtos)
                {
                    if (!dictionary.ContainsKey(dto.SKU) || !dictionary[dto.SKU].ContainsKey(dto.WarehouseId))
                        continue;

                    WarehouseStock stock = dictionary[dto.SKU][dto.WarehouseId];
                    if (stock.StockLevel == dto.StockLevel)
                        continue;

                    stock.StockLevel = dto.StockLevel;
                    stockToUpdate.Add(stock);
                }
                foreach (WarehouseStock stock in stockToUpdate)
                {
                    _statelessSession.Update(stock);
                }
                transaction.Commit();
            }

            return new BulkUpdateResult
            {
                IsSuccess = true,
                Messages = new List<string>
                {
                    string.Format(
                        _stringResourceProvider.GetValue("Bulk Stock Update - number processed", "{0} items processed"),
                        dtos.Count()),
                    string.Format(
                        _stringResourceProvider.GetValue("Bulk Stock Update - number updated", "{0} items updated"),
                        stockToUpdate.Count)
                }
            };
        }
    }
}