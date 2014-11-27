using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public class ImportOptions : IImportOptions
    {
        private readonly ISession _session;

        public ImportOptions(ISession session)
        {
            _session = session;
        }

        public string ProcessOptions(INopCommerceProductReader nopCommerceProductReader, string connectionString,
            NopImportContext nopImportContext)
        {
            List<ProductOptionData> productOptionDatas = nopCommerceProductReader.GetProductOptions(connectionString);

            _session.Transact(session =>
            {
                foreach (ProductOptionData productOptionData in productOptionDatas)
                {
                    var productOption = new ProductOption
                    {
                        Name = productOptionData.Name
                    };
                    session.Save(productOption);
                    nopImportContext.AddEntry(productOptionData.Id, productOption);
                }
            });

            return string.Format("{0} product options processed", productOptionDatas.Count);
        }
    }
}