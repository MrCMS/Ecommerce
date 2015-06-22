using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors;
using StackExchange.Profiling;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public class PerformNopImport : IPerformNopImport
    {
        private readonly IImportBrands _importBrands;
        private readonly IImportCategories _importCategories;
        private readonly IImportPictureData _importPictureData;
        private readonly IImportCountryData _importCountryData;
        private readonly IImportOptions _importOptions;
        private readonly IImportProducts _importProducts;
        private readonly IImportOrders _importOrders;
        private readonly IImportRegionData _importRegionData;
        private readonly IImportUsers _importUsers;
        private readonly IImportSpecificationAttributeOptions _importSpecificationAttributeOptions;
        private readonly IImportSpecifications _importSpecifications;
        private readonly IImportTags _importTags;
        private readonly IImportTaxRates _importTaxRates;

        private readonly IIndexService _indexService;
        private readonly IImportAddresses _importAddresses;

        public PerformNopImport(
            IImportPictureData importPictureData,
            IImportCountryData importCountryData,
            IImportRegionData importRegionData,
            IImportUsers importUsers,
            IImportTaxRates importTaxRates,
            IImportBrands importBrands,
            IImportSpecifications importSpecifications,
            IImportSpecificationAttributeOptions importSpecificationAttributeOptions,
            IImportOptions importOptions,
            IImportTags importTags,
            IImportCategories importCategories,
            IImportProducts importProducts,
            IImportOrders importOrders,
            IIndexService indexService, IImportAddresses importAddresses)
        {
            _importPictureData = importPictureData;
            _importCountryData = importCountryData;
            _importRegionData = importRegionData;
            _importUsers = importUsers;
            _importTaxRates = importTaxRates;
            _importBrands = importBrands;
            _importSpecifications = importSpecifications;
            _importSpecificationAttributeOptions = importSpecificationAttributeOptions;
            _importOptions = importOptions;
            _importTags = importTags;
            _importCategories = importCategories;
            _importProducts = importProducts;
            _importOrders = importOrders;
            _indexService = indexService;
            _importAddresses = importAddresses;
        }

        public ImportResult Execute(NopCommerceDataReader dataReader)
        {
            using (new NotificationDisabler())
            using (EventContext.Instance.DisableAll())
            {
                var nopImportContext = new NopImportContext();
                var messages = new List<string>();
                using (MiniProfiler.Current.Step("Import Pictures"))
                    messages.Add(_importPictureData.ImportPictures(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Countries"))
                    messages.Add(_importCountryData.ProcessCountries(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Regions"))
                    messages.Add(_importRegionData.ProcessRegions(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Addresses"))
                    messages.Add(_importAddresses.ProcessAddresses(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Users"))
                    messages.Add(_importUsers.ProcessUsers(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Tax Rates"))
                    messages.Add(_importTaxRates.ProcessTaxRates(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Brands"))
                    messages.Add(_importBrands.ProcessBrands(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Specifications"))
                    messages.Add(_importSpecifications.ProcessSpecifications(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Specification Options"))
                    messages.Add(_importSpecificationAttributeOptions.ProcessSpecificationAttributeOptions(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Options"))
                    messages.Add(_importOptions.ProcessOptions(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Tags"))
                    messages.Add(_importTags.ProcessTags(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Categories"))
                    messages.Add(_importCategories.ProcessCategories(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Products"))
                    messages.Add(_importProducts.ProcessProducts(dataReader, nopImportContext));
                using (MiniProfiler.Current.Step("Import Orders"))
                    messages.Add(_importOrders.ProcessOrders(dataReader, nopImportContext));

                _indexService.InitializeAllIndices();
                return new ImportResult
                {
                    Messages = messages,
                    Success = true
                };
            }
        }

        public ImportResult UpdateOrdersAndUsers(NopCommerceDataReader dataReader)
        {
            using (new NotificationDisabler())
            {
                var nopImportContext = new NopImportContext();
                var messages = new List<string>
                {
                    _importUsers.ProcessUsers(dataReader, nopImportContext),
                    _importOrders.ProcessOrders(dataReader, nopImportContext)
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