using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public class ImportSpecificationAttributeOptions : IImportSpecificationAttributeOptions
    {
        private readonly ISession _session;

        public ImportSpecificationAttributeOptions(ISession session)
        {
            _session = session;
        }

        public string ProcessSpecificationAttributeOptions(INopCommerceProductReader nopCommerceProductReader, string connectionString,
            NopImportContext nopImportContext)
        {
            List<ProductSpecificationAttributeOptionData> optionDatas =
                nopCommerceProductReader.GetProductSpecificationOptions(connectionString);

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
                    session.Save(option);
                    nopImportContext.AddEntry(optionData.Id, option);
                }
            });
            return string.Format("{0} product specification attribute options processed", optionDatas.Count);

        }
    }
}