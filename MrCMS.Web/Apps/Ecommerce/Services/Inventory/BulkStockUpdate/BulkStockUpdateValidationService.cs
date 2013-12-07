using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.Rules;
using MrCMS.Website;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public class BulkStockUpdateValidationService : IBulkStockUpdateValidationService
    {
        public Dictionary<string, List<string>> ValidateBusinessLogic(IEnumerable<BulkStockUpdateDataTransferObject> items)
        {
            var errors = new Dictionary<string, List<string>>();
            var productVariantRules = MrCMSApplication.GetAll<IBulkStockUpdateValidationRule>();

            foreach (var item in items)
            {
                var productVariantErrors = productVariantRules.SelectMany(rule => rule.GetErrors(item)).ToList();
                if (productVariantErrors.Any())
                    errors.Add(item.SKU, productVariantErrors);
            }

            return errors;
        }

        public List<BulkStockUpdateDataTransferObject> ValidateAndBulkStockUpdateProductVariants(Stream rawFile, ref Dictionary<string, List<string>> parseErrors)
        {
            var items = new List<BulkStockUpdateDataTransferObject>();

            if (rawFile != null)
            {
                using (var file = new CsvReader(new StreamReader(rawFile)))
                {
                    while (file.Read())
                    {
                        string sku = file.GetField<string>(1),
                               name = file.GetField<string>(0);
                        var handle = sku.HasValue() ? sku : SeoHelper.TidyUrl(name);
                        if (parseErrors.All(x => x.Key != handle))
                            parseErrors.Add(handle, new List<string>());

                        var pv = new BulkStockUpdateDataTransferObject
                            {
                                Name = file.GetField<string>(0),
                                SKU = file.GetField<string>(1)
                            };

                        if (file.GetField<string>(1).HasValue())
                            pv.SKU = file.GetField<string>(1);
                        else
                            parseErrors[handle].Add("SKU is required.");

                        if (!GeneralHelper.IsValidInput<int>(file.GetField<string>(2)))
                            parseErrors[handle].Add("Stock value is not a valid number.");
                        else
                            pv.StockRemaining = file.GetField<string>(2).HasValue()
                                           ? Int32.Parse(file.GetField<string>(2))
                                           : 0;

                        items.Add(pv);
                    }
                }
            }

            parseErrors = parseErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);
            
            return items;
        }
    }
}