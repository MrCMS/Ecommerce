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
            IEnumerable<INopCommerceProductReader> nopCommerceProductReaders = GetNopCommerceProductReaders();

            return nopCommerceProductReaders.BuildSelectItemList(reader => reader.Name,
                reader => reader.GetType().FullName, emptyItem: null);
        }

        public ImportResult Import(ImportParams importParams)
        {
            INopCommerceProductReader nopCommerceProductReader =
                GetNopCommerceProductReaders().FirstOrDefault(x => x.GetType().FullName == importParams.ImporterType);

            if (nopCommerceProductReader == null)
                return new ImportResult {Messages = new List<string> {"Could not find the requested importer"}};

            return _performNopImport.Execute(nopCommerceProductReader, importParams.ConnectionString);
        }

        private IEnumerable<INopCommerceProductReader> GetNopCommerceProductReaders()
        {
            return _kernel.GetAll<INopCommerceProductReader>();
        }
    }
}