using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class AvailableBrandsService : IAvailableBrandsService
    {
        private readonly IProductSearchIndexService _productSearchIndexService;
        private readonly ISession _session;

        public AvailableBrandsService(ISession session, IProductSearchIndexService productSearchIndexService)
        {
            _session = session;
            _productSearchIndexService = productSearchIndexService;
        }


        public List<SelectListItem> GetAvailableBrands(ProductSearchQuery query)
        {
            BrandInfo info = null;
            List<int> brands = _productSearchIndexService.GetBrands(query);
            IList<BrandInfo> items =
                _session.QueryOver<BrandPage>().Where(item => item.Id.IsIn(brands)).SelectList(builder =>
                {
                    builder.Select(x => x.Id).WithAlias(() => info.Id);
                    builder.Select(x => x.Name).WithAlias(() => info.Name);
                    return builder;
                }).TransformUsing(Transformers.AliasToBean<BrandInfo>())
                    .List<BrandInfo>();
            return items.BuildSelectItemList(brand => brand.Name, brand => brand.Id.ToString(),
                brand => brand.Id == query.BrandId, "All Brands");
        }

        public class BrandInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}