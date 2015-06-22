using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportSpecifications : IImportSpecifications
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportSpecifications(IStatelessSession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessSpecifications(NopCommerceDataReader dataReader,
            NopImportContext nopImportContext)
        {
            var productSpecificationDatas = dataReader.GetProductSpecifications();
            var site = _session.Get<Site>(_site.Id);
            _session.Transact(session =>
            {
                foreach (ProductSpecificationData productSpecificationData in productSpecificationDatas)
                {
                    var specificationAttribute = new ProductSpecificationAttribute
                    {
                        Name = productSpecificationData.Name,
                    };
                    specificationAttribute.AssignBaseProperties(site);
                    session.Insert(specificationAttribute);
                    nopImportContext.AddEntry(productSpecificationData.Id, specificationAttribute);
                }
            });
            return string.Format("{0} product specifications processed", productSpecificationDatas.Count);
        }
    }
}