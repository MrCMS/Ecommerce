using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public class BulkSimpleStockUpdateValidationService : IBulkSimpleStockUpdateValidationService
    {
        public GetProductVariantsFromFileResult ValidateFile(Stream rawFile)
        {
            var items = new List<BulkStockUpdateDataTransferObject>();
            var errors = new List<string>();

            if (rawFile != null)
            {
                using (var file = new CsvReader(new StreamReader(rawFile)))
                {
                    while (file.Read())
                    {
                        if (file.FieldHeaders.Length == 4)
                        {
                            return GetProductVariantsFromFileResult.Failure(new List<string>
                            {
                                "You have attempted to import a file in the warehoused stock format into the simple stock updater."
                            });
                        }
                        string sku = file.GetField<string>(1),
                            name = file.GetField<string>(0);
                        string handle = name ?? sku;

                        var pv = new BulkStockUpdateDataTransferObject
                        {
                            Name = name,
                            SKU = sku
                        };

                        if (sku.HasValue())
                            pv.SKU = sku;
                        else
                            errors.Add(string.Format("SKU is required for {0}.", handle));

                        var stockValue = file.GetField<string>(2);
                        int stockRemaining;
                        if (int.TryParse(stockValue, out stockRemaining))
                            pv.StockRemaining = stockRemaining;
                        else
                            errors.Add(string.Format("Stock value for {0} is not a valid number.", handle));

                        items.Add(pv);
                    }
                }
            }

            return errors.Any()
                ? GetProductVariantsFromFileResult.Failure(errors)
                : GetProductVariantsFromFileResult.Success(items);
        }

        public List<BulkStockUpdateDataTransferObject> ValidateAndBulkStockUpdateProductVariants(Stream rawFile,
            ref Dictionary<string, List<string>> parseErrors)
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
                        string handle = sku.HasValue() ? sku : SeoHelper.TidyUrl(name);
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