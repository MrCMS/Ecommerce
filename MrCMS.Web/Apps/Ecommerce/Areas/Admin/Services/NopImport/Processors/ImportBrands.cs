using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportBrands : IImportBrands
    {
        private readonly IGetNewBrandPage _getNewBrandPage;
        private readonly Site _site;
        private readonly IStatelessSession _session;

        public ImportBrands(IStatelessSession session, IGetNewBrandPage getNewBrandPage, Site site)
        {
            _session = session;
            _getNewBrandPage = getNewBrandPage;
            _site = site;
        }

        public string ProcessBrands(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            HashSet<BrandData> brandDatas = dataReader.GetBrands();
            var brands = _session.QueryOver<Brand>().List().ToDictionary(x => x.Name);
            var site = _session.Get<Site>(_site.Id);
            foreach (BrandData brandData in brandDatas)
            {
                string name = brandData.Name.Trim();
                Brand brand;
                if (!brands.ContainsKey(name))
                {
                    brand = _getNewBrandPage.Get(name);
                    brand.AssignBaseProperties(site);
                    _session.Transact(session => session.Insert(brand));
                }
                else
                {
                    brand = brands[name];
                }
                nopImportContext.AddEntry(brandData.Id, brand);
            }
            return string.Format("{0} brands processed", brandDatas.Count);
        }
    }
}