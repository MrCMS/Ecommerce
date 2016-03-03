using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportOptions : IImportOptions
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportOptions(IStatelessSession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessOptions(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var productOptionDatas = dataReader.GetProductOptions();
            var site = _session.Get<Site>(_site.Id);

            _session.Transact(session =>
            {
                foreach (ProductOptionData productOptionData in productOptionDatas)
                {
                    var productOption = new ProductOption
                    {
                        Name = productOptionData.Name
                    };
                    productOption.AssignBaseProperties(site);
                    session.Insert(productOption);
                    nopImportContext.AddEntry(productOptionData.Id, productOption);
                }
            });

            return string.Format("{0} product options processed", productOptionDatas.Count);
        }
    }
}