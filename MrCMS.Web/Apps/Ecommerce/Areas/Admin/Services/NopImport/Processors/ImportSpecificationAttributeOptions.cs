using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportSpecificationAttributeOptions : IImportSpecificationAttributeOptions
    {
        private readonly ISession _session;

        public ImportSpecificationAttributeOptions(ISession session)
        {
            _session = session;
        }

        public string ProcessSpecificationAttributeOptions(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var optionDatas = dataReader.GetProductSpecificationOptions();

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