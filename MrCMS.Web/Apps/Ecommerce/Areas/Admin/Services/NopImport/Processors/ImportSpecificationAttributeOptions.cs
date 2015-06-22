using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportSpecificationAttributeOptions : IImportSpecificationAttributeOptions
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportSpecificationAttributeOptions(IStatelessSession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessSpecificationAttributeOptions(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var optionDatas = dataReader.GetProductSpecificationOptions();
            var site = _session.Get<Site>(_site.Id);
            _session.Transact(session =>
            {
                foreach (ProductSpecificationAttributeOptionData optionData in optionDatas)
                {
                    var option = new ProductSpecificationAttributeOption
                    {
                        Name = optionData.Name,
                        ProductSpecificationAttribute =
                            nopImportContext.FindNew<ProductSpecificationAttribute>(optionData.ProductSpecificationId)
                    };
                    option.AssignBaseProperties(site);
                    session.Insert(option);
                    nopImportContext.AddEntry(optionData.Id, option);
                }
            });
            return string.Format("{0} product specification attribute options processed", optionDatas.Count);

        }
    }
}