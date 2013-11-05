using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.Rules;
using MrCMS.Website;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate
{
    public class BulkShippingUpdateValidationService : IBulkShippingUpdateValidationService
    {
        public Dictionary<string, List<string>> ValidateBusinessLogic(IEnumerable<BulkShippingUpdateDataTransferObject> items)
        {
            var errors = new Dictionary<string, List<string>>();
            var rules = MrCMSApplication.GetAll<IBulkShippingUpdateValidationRule>();

            foreach (var item in items)
            {
                var ruleErrors = rules.SelectMany(rule => rule.GetErrors(item)).ToList();
                if (ruleErrors.Any())
                    errors.Add(item.OrderId.ToString(), ruleErrors);
            }

            return errors;
        }

        public List<BulkShippingUpdateDataTransferObject> ValidateAndBulkShippingUpdateOrders(Stream rawFile, ref Dictionary<string, List<string>> parseErrors)
        {
            var bulkShippingUpdateDataTransferObjects = new List<BulkShippingUpdateDataTransferObject>();

            if (rawFile != null)
            {
                using (var file = new CsvReader(new StreamReader(rawFile), new CsvConfiguration{HasHeaderRecord = true}))
                {
                    while (file.Read())
                    {
                        var orderId = file.GetField<int?>(0);

                        //skip blank rows
                        if (!orderId.HasValue)
                            continue;
                        
                        //check for duplicates
                        if (bulkShippingUpdateDataTransferObjects.SingleOrDefault(x=>x.OrderId == orderId) != null)
                            continue;

                        if (orderId.HasValue && parseErrors.All(x => x.Key != orderId.ToString()))
                            parseErrors.Add(orderId.ToString(), new List<string>());
                        else
                            parseErrors.Add("no-supplied-id", new List<string>());

                        var pv = new BulkShippingUpdateDataTransferObject();

                        if (file.GetField<string>(0).HasValue())
                            pv.OrderId = file.GetField<int>(0);
                        else
                            parseErrors["no-supplied-id"].Add("Order Id is required.");

                        if (file.GetField<string>(1).HasValue())
                            pv.ShippingMethod = file.GetField<string>(1);

                        if (file.GetField<string>(2).HasValue())
                            pv.TrackingNumber = file.GetField<string>(2).Trim();

                        bulkShippingUpdateDataTransferObjects.Add(pv);
                    }
                }
            }

            parseErrors = parseErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);
            
            return bulkShippingUpdateDataTransferObjects;
        }
    }
}