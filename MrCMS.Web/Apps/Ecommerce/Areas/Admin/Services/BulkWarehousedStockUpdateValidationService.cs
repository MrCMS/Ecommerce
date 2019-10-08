using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class BulkWarehousedStockUpdateValidationService : IBulkWarehousedStockUpdateValidationService
    {
        public GetWarehouseStockFromFileResult ValidateFile(Stream rawFile)
        {
            var items = new List<BulkWarehouseStockUpdateDTO>();
            var errors = new List<string>();
            if (rawFile != null)
            {
                try
                {
                    using (var file = new CsvReader(new StreamReader(rawFile)))
                    {
                        while (file.Read())
                        {
                            string sku = file.GetField<string>(1).Trim(),
                                name = file.GetField<string>(0);
                            var handle = name ?? sku;

                            var dto = new BulkWarehouseStockUpdateDTO
                            {
                                SKU = sku
                            };

                            if (!string.IsNullOrWhiteSpace(sku))
                                dto.SKU = sku;
                            else
                                errors.Add(string.Format("SKU is required for {0}.", handle));

                            var warehouseIdValue = file.GetField<string>(2);
                            int warehouseId;
                            if (int.TryParse(warehouseIdValue, out warehouseId))
                                dto.WarehouseId = warehouseId;
                            else
                                errors.Add(string.Format("Warehouse id for {0} is not a valid number.", handle));

                            var stockLevelValue = file.GetField<string>(3);
                            int stockLevel;
                            if (int.TryParse(stockLevelValue, out stockLevel))
                                dto.StockLevel = stockLevel;
                            else
                                errors.Add(string.Format("Stock level for {0} is not a valid number.", handle));

                            items.Add(dto);
                        }
                    }

                    var duplicates = items
                        .GroupBy(x => new {x.SKU, x.WarehouseId})
                        .Where(group => group.Count() > 1)
                        .Select(group => new BulkWarehouseStockUpdateDTO
                        {
                            SKU = group.Key.SKU,
                            WarehouseId = group.Key.WarehouseId
                        }).ToList();

                    if (duplicates.Any())
                    {
                        foreach (var bulkWarehouseStockUpdateDto in duplicates)
                        {
                            errors.Add($"A duplicate SKU of {bulkWarehouseStockUpdateDto.SKU} was found for warehouse Id {bulkWarehouseStockUpdateDto.WarehouseId}");
                        }

                    }
                }
                catch (CsvMissingFieldException exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                    return GetWarehouseStockFromFileResult.Failure(new List<string>
                    {
                        "A field was missing from your import. This likely means you tried to import a simple stock list with the warehoused stock enabled."
                    });
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                    return GetWarehouseStockFromFileResult.Failure(new List<string>
                    {
                        "An error occurred during import, check the logs for exception details"
                    });
                }
            }

            return errors.Any()
                ? GetWarehouseStockFromFileResult.Failure(errors)
                : GetWarehouseStockFromFileResult.Success(items);
        }
    }
}