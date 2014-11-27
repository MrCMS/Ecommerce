using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Services.Notifications;
using MrCMS.Web.Areas.Admin.Services.NopImport.Processors;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public class PerformNopImport : IPerformNopImport
    {
        private readonly IImportBrands _importBrands;
        private readonly IImportCategories _importCategories;
        private readonly IImportCountryData _importCountryData;
        private readonly IImportOptions _importOptions;
        private readonly IImportProducts _importProducts;
        private readonly IImportRegionData _importRegionData;
        private readonly IImportSpecificationAttributeOptions _importSpecificationAttributeOptions;
        private readonly IImportSpecifications _importSpecifications;
        private readonly IImportTags _importTags;
        private readonly IImportTaxRates _importTaxRates;

        private readonly IIndexService _indexService;

        public PerformNopImport(
            IImportCountryData importCountryData,
            IImportRegionData importRegionData,
            IImportTaxRates importTaxRates,
            IImportBrands importBrands,
            IImportSpecifications importSpecifications,
            IImportSpecificationAttributeOptions importSpecificationAttributeOptions,
            IImportOptions importOptions,
            IImportTags importTags,
            IImportCategories importCategories,
            IImportProducts importProducts,
            IIndexService indexService)
        {
            _importCountryData = importCountryData;
            _importRegionData = importRegionData;
            _importTaxRates = importTaxRates;
            _importBrands = importBrands;
            _importSpecifications = importSpecifications;
            _importSpecificationAttributeOptions = importSpecificationAttributeOptions;
            _importOptions = importOptions;
            _importTags = importTags;
            _importCategories = importCategories;
            _importProducts = importProducts;
            _indexService = indexService;
        }

        public ImportResult Execute(INopCommerceProductReader nopCommerceProductReader, string connectionString)
        {
            using (new NotificationDisabler())
            {
                var nopImportContext = new NopImportContext();
                var messages = new List<string>
                {
                    _importCountryData.ProcessCountries(nopCommerceProductReader, connectionString, nopImportContext),
                    _importRegionData.ProcessRegions(nopCommerceProductReader, connectionString, nopImportContext),
                    _importTaxRates.ProcessTaxRates(nopCommerceProductReader, connectionString, nopImportContext),
                    _importBrands.ProcessBrands(nopCommerceProductReader, connectionString, nopImportContext),
                    _importSpecifications.ProcessSpecifications(nopCommerceProductReader, connectionString, nopImportContext),
                    _importSpecificationAttributeOptions.ProcessSpecificationAttributeOptions( nopCommerceProductReader, connectionString, nopImportContext),
                    _importOptions.ProcessOptions(nopCommerceProductReader, connectionString, nopImportContext),
                    _importTags.ProcessTags(nopCommerceProductReader, connectionString, nopImportContext),
                    _importCategories.ProcessCategories(nopCommerceProductReader, connectionString, nopImportContext),
                    _importProducts.ProcessProducts(nopCommerceProductReader, connectionString, nopImportContext)
                };

                _indexService.InitializeAllIndices();
                return new ImportResult
                {
                    Messages = messages,
                    Success = true
                };
            }
        }
    }
}