using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class WarehouseAdminService : IWarehouseAdminService
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;

        public WarehouseAdminService(ISession session, IStringResourceProvider stringResourceProvider)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
        }

        public IPagedList<Warehouse> Search(WarehouseSearchModel searchModel)
        {
            IQueryOver<Warehouse, Warehouse> queryOver = _session.QueryOver<Warehouse>();

            return queryOver.Paged(searchModel.Page);
        }

        public void Add(Warehouse warehouse)
        {
            _session.Transact(session => session.Save(warehouse));
        }

        public void Update(Warehouse warehouse)
        {
            _session.Transact(session => session.Update(warehouse));
        }

        public void Delete(Warehouse warehouse)
        {
            _session.Transact(session => session.Delete(warehouse));
        }

        public StockGenerationModel GetStockGenerationModel(Warehouse warehouse)
        {
            return new StockGenerationModel
            {
                WarehouseId = warehouse.Id
            };
        }

        public List<SelectListItem> GetWarehouseOptions(Warehouse warehouse)
        {
            IList<Warehouse> warehouses = _session.QueryOver<Warehouse>()
                .Where(w => w.Id != warehouse.Id)
                .OrderBy(w => w.Name).Asc
                .Cacheable().List();

            return warehouses.BuildSelectItemList(w => w.Name,
                w => w.Id.ToString(),
                emptyItem: null);
        }

        public List<SelectListItem> GetStockGenerationTypeOptions(Warehouse warehouse)
        {
            IList<Warehouse> warehouses = _session.QueryOver<Warehouse>()
                .Where(w => w.Id != warehouse.Id)
                .OrderBy(w => w.Name).Asc
                .Cacheable().List();
            List<StockGenerationType> stockGenerationOptions =
                Enum.GetValues(typeof(StockGenerationType)).Cast<StockGenerationType>().ToList();

            if (!warehouses.Any())
                stockGenerationOptions.RemoveAll(item => item == StockGenerationType.CopyFromWarehouse);
            else
                stockGenerationOptions.RemoveAll(item => item == StockGenerationType.CopyFromSystemValues);

            return stockGenerationOptions.BuildSelectItemList(type => type.ToString().BreakUpString(),
                type => type.ToString(), emptyItem: null);
        }

        public bool AnyStock(Warehouse warehouse)
        {
            return _session.QueryOver<WarehouseStock>().Where(stock => stock.Warehouse.Id == warehouse.Id).Any();
        }

        private const string ResourcePrefix = "Generate Stock - ";
        public GenerateStockResult GenerateStock(StockGenerationModel model)
        {
            var warehouse = _session.Get<Warehouse>(model.WarehouseId);
            if (warehouse == null)
            {
                var message = _stringResourceProvider.GetValue(string.Format("{0}Warehouse not found", ResourcePrefix), "Cannot find the warehouse to add stock to");
                return GenerateStockResult.Failure(message);
            }
            var productVariantsToTrack =
                _session.QueryOver<ProductVariant>()
                    .Where(variant => variant.TrackingPolicy == TrackingPolicy.Track)
                    .Cacheable()
                    .List()
                    .ToHashSet();
            var newWarehouseStocks = productVariantsToTrack.Select(variant => new WarehouseStock
            {
                ProductVariant = variant,
                Warehouse = warehouse
            }).ToHashSet();
            switch (model.StockGenerationType)
            {
                case StockGenerationType.FixedValue:
                    newWarehouseStocks.ForEach(stock => stock.StockLevel = model.FixedValue);
                    break;
                case StockGenerationType.CopyFromSystemValues:
                    newWarehouseStocks.ForEach(stock => stock.StockLevel = stock.ProductVariant.StockRemaining);
                    break;
                case StockGenerationType.CopyFromWarehouse:
                    var stockLevels = _session.QueryOver<WarehouseStock>()
                        .Where(stock => stock.Warehouse.Id == model.WarehouseToCopyId)
                        .Cacheable()
                        .List()
                        .ToDictionary(stock => stock.ProductVariant.Id, stock => stock.StockLevel);
                    newWarehouseStocks.ForEach(stock =>
                    {
                        stock.StockLevel = stockLevels.ContainsKey(stock.ProductVariant.Id)
                            ? stockLevels[stock.ProductVariant.Id]
                            : 0;
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _session.Transact(session => newWarehouseStocks.ForEach(stock => session.Save(stock)));
            return GenerateStockResult.Success("Stock generated successfully");
        }

        public IList<Warehouse> ListAll()
        {
            return _session.QueryOver<Warehouse>().OrderBy(warehouse => warehouse.Name).Asc.Cacheable().List();
        }
    }
}