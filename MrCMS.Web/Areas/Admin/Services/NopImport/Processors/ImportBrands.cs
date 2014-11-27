using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public class ImportBrands : IImportBrands
    {
        private readonly ISession _session;

        public ImportBrands(ISession session)
        {
            _session = session;
        }

        public string ProcessBrands(INopCommerceProductReader nopCommerceProductReader, string connectionString,
            NopImportContext nopImportContext)
        {
            List<BrandData> brandDatas = nopCommerceProductReader.GetBrands(connectionString);
            foreach (BrandData brandData in brandDatas)
            {
                string name = brandData.Name.Trim();
                Brand brand =
                    _session.QueryOver<Brand>()
                        .Where(b => b.Name.IsInsensitiveLike(name, MatchMode.Exact))
                        .List().FirstOrDefault();
                if (brand == null)
                {
                    brand = new Brand { Name = name };
                    _session.Transact(session => session.Save(brand));
                }
                nopImportContext.AddEntry(brandData.Id, brand);
            }
            return string.Format("{0} brands processed", brandDatas.Count);
        }
    }
}