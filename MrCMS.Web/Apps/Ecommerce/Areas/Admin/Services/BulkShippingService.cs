using System.Collections.Generic;
using System.IO;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class BulkShippingService : IBulkShippingService
    {
        private readonly IBulkShippingUpdateService _bulkShippingUpdateService;
        private readonly IBulkShippingUpdateValidationService _bulkShippingUpdateValidationService;

        public BulkShippingService(IBulkShippingUpdateValidationService bulkShippingUpdateValidationService,
            IBulkShippingUpdateService bulkShippingUpdateService)
        {
            _bulkShippingUpdateValidationService = bulkShippingUpdateValidationService;
            _bulkShippingUpdateService = bulkShippingUpdateService;
        }

        public Dictionary<string, List<string>> Update(Stream file)
        {
            Dictionary<string, List<string>> parseErrors;
            List<BulkShippingUpdateDataTransferObject> items = GetOrdersFromFile(file, out parseErrors);
            if (parseErrors.Any())
                return parseErrors;
            Dictionary<string, List<string>> businessLogicErrors =
                _bulkShippingUpdateValidationService.ValidateBusinessLogic(items);
            if (businessLogicErrors.Any())
                return businessLogicErrors;
            int noOfUpdatedItems = _bulkShippingUpdateService.BulkShippingUpdateFromDTOs(items);
            return new Dictionary<string, List<string>>
            {
                {
                    "success", new List<string>
                    {
                        noOfUpdatedItems > 0
                            ? noOfUpdatedItems +
                              (noOfUpdatedItems > 1 ? " items were" : " item was") +
                              " successfully updated."
                            : "No items were updated."
                    }
                }
            };
        }

        private List<BulkShippingUpdateDataTransferObject> GetOrdersFromFile(Stream file,
            out Dictionary<string, List<string>>
                parseErrors)
        {
            parseErrors = new Dictionary<string, List<string>>();
            return _bulkShippingUpdateValidationService.ValidateAndBulkShippingUpdateOrders(file, ref parseErrors);
        }
    }
}