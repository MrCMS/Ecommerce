using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;
using Brand = MrCMS.Web.Apps.Ecommerce.Pages.Brand;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportBrands : IImportBrands
    {
        private readonly ISession _session;
        private readonly IGetNewBrandPage _getNewBrandPage;

        public ImportBrands(ISession session,IGetNewBrandPage getNewBrandPage)
        {
            _session = session;
            _getNewBrandPage = getNewBrandPage;
        }

        public string ProcessBrands(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var brandDatas = dataReader.GetBrands();
            foreach (BrandData brandData in brandDatas)
            {
                string name = brandData.Name.Trim();
                Brand brand =
                    _session.QueryOver<Brand>()
                        .Where(b => b.Name.IsInsensitiveLike(name, MatchMode.Exact))
                        .List().FirstOrDefault();
                if (brand == null)
                {
                    brand = _getNewBrandPage.Get(name);
                    _session.Transact(session => session.Save(brand));
                }
                nopImportContext.AddEntry(brandData.Id, brand);
            }
            return string.Format("{0} brands processed", brandDatas.Count);
        }
    }
}