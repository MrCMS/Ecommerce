using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class WarehouseStockAdminService : IWarehouseStockAdminService
    {
        private readonly ISession _session;

        public WarehouseStockAdminService(ISession session)
        {
            _session = session;
        }

        public IPagedList<WarehouseStock> Search(WarehouseStockSearchModel stockSearchModel)
        {
            ProductVariant productVariantAlias = null;
            Product productAlias = null;
            var queryOver = _session.QueryOver<WarehouseStock>()
                .JoinAlias(stock => stock.ProductVariant, () => productVariantAlias)
                .JoinAlias(() => productVariantAlias.Product, () => productAlias)
                .Fetch(stock => stock.ProductVariant).Eager
                .Fetch(stock => stock.Warehouse).Eager;

            if (stockSearchModel.WarehouseId.HasValue)
            {
                queryOver = queryOver.Where(stock => stock.Warehouse.Id == stockSearchModel.WarehouseId);
            }
            if (!string.IsNullOrWhiteSpace(stockSearchModel.SKU))
            {
                queryOver =
                    queryOver.Where(
                        () => productVariantAlias.SKU == stockSearchModel.SKU);
            }
            if (!string.IsNullOrWhiteSpace(stockSearchModel.Name))
            {
                queryOver =
                    queryOver.Where(
                        () => productVariantAlias.Name.IsInsensitiveLike(stockSearchModel.Name, MatchMode.Anywhere) ||
                              productAlias.Name.IsInsensitiveLike(stockSearchModel.Name, MatchMode.Anywhere)
                        );
            }

            return queryOver.Paged(stockSearchModel.Page);
        }

        public List<SelectListItem> GetWarehouseOptions()
        {
            var warehouses = _session.QueryOver<Warehouse>().OrderBy(warehouse => warehouse.Name).Asc
                .Cacheable().List();

            return warehouses.BuildSelectItemList(warehouse => warehouse.Name,
                warehouse => warehouse.Id.ToString(), emptyItemText: "Any");
        }

        public void Update(WarehouseStock stock)
        {
            _session.Transact(session => session.Update(stock));
        }
    }
}