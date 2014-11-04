using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ProductVariantAdminService : IProductVariantAdminService
    {
        private readonly IProductVariantAdminViewDataService _productVariantAdminViewDataService;
        private readonly ISession _session;

        public ProductVariantAdminService(IProductVariantAdminViewDataService productVariantAdminViewDataService, ISession session)
        {
            _productVariantAdminViewDataService = productVariantAdminViewDataService;
            _session = session;
        }

        public void SetViewData(ViewDataDictionary viewData, ProductVariant productVariant)
        {
            _productVariantAdminViewDataService.SetViewData(viewData, productVariant);
        }

        public void Add(ProductVariant productVariant)
        {
            _session.Transact(session => session.Save(productVariant));
        }

        public void Delete(ProductVariant productVariant)
        {
            _session.Transact(session => session.Delete(productVariant));
        }

        public bool AnyExistingProductVariantWithSKU(string sku, int id)
        {
            var trim = sku.Trim();
            return _session.QueryOver<ProductVariant>().Where(variant => variant.SKU == trim && variant.Id != id).Any();
        }

        public void Update(ProductVariant productVariant)
        {
            _session.Transact(session => session.SaveOrUpdate(productVariant));
        }
    }
}