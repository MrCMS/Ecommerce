using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors;

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
            {
                var nopImportContext = new NopImportContext();
                var messages = new List<string>
                {
                    _importPictureData.ImportPictures(dataReader,nopImportContext),
                    _importCountryData.ProcessCountries(dataReader, nopImportContext),
                    _importRegionData.ProcessRegions(dataReader, nopImportContext),
                    _importAddresses.ProcessAddresses(dataReader, nopImportContext),
                    _importUsers.ProcessUsers(dataReader, nopImportContext),
                    //_importTaxRates.ProcessTaxRates(dataReader, nopImportContext),
                    //_importBrands.ProcessBrands(dataReader, nopImportContext),
                    //_importSpecifications.ProcessSpecifications(dataReader, nopImportContext),
                    //_importSpecificationAttributeOptions.ProcessSpecificationAttributeOptions(dataReader, nopImportContext),
                    //_importOptions.ProcessOptions(dataReader, nopImportContext),
                    //_importTags.ProcessTags(dataReader, nopImportContext),
                    //_importCategories.ProcessCategories(dataReader, nopImportContext),
                    //_importProducts.ProcessProducts(dataReader, nopImportContext),
                    //_importOrders.ProcessOrders(dataReader,nopImportContext)
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