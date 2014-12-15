using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportSpecifications : IImportSpecifications
    {
        private readonly ISession _session;

        public ImportSpecifications(ISession session)
        {
            _session = session;
        }

        public string ProcessSpecifications(NopCommerceDataReader dataReader,
            NopImportContext nopImportContext)
        {
            var productSpecificationDatas = dataReader.GetProductSpecifications();

            _session.Transact(session =>
            {
                foreach (ProductSpecificationData productSpecificationData in productSpecificationDatas)
                {
                    var specificationAttribute = new ProductSpecificationAttribute
                    {
                        Name = productSpecificationData.Name,
                    };
                    session.Save(specificationAttribute);
                    nopImportContext.AddEntry(productSpecificationData.Id, specificationAttribute);
                }
            });
            return string.Format("{0} product specifications processed", productSpecificationDatas.Count);
        }
    }
}