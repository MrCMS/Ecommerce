using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public class NopProductImportAdminService : INopProductImportAdminService
    {
        private readonly IKernel _kernel;
        private readonly IPerformNopImport _performNopImport;

        public NopProductImportAdminService(IKernel kernel, IPerformNopImport performNopImport)
        {
            _kernel = kernel;
            _performNopImport = performNopImport;
        }

        public List<SelectListItem> GetImporterOptions()
        {
            IEnumerable<NopCommerceDataReader> nopCommerceDataReaders = GetNopCommerceDataReaders();

            return nopCommerceDataReaders.BuildSelectItemList(reader => reader.Name,
                reader => reader.GetType().FullName, emptyItem: null);
        }

        public ImportResult Import(ImportParams importParams)
        {
            var reader = GetNopCommerceDataReaders().FirstOrDefault(x => x.GetType().FullName == importParams.ImporterType);

            if (reader== null)
                return new ImportResult { Messages = new List<string> { "Could not find the requested importer" } };

            reader.SetConnectionString(importParams.ConnectionString);
            return _performNopImport.Execute(reader);
        }

        private IEnumerable<NopCommerceDataReader> GetNopCommerceDataReaders()
        {
            return _kernel.GetAll<INopCommerceDataReader>().OfType<NopCommerceDataReader>();
        }
    }
}