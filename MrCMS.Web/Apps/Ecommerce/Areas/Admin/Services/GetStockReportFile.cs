using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GetStockReportFile : IGetStockReportFile
    {
        private readonly ICSVFileWriter _csvFileWriter;
        private readonly IGetStockRemainingQuantity _getStockRemainingQuantity;

        public GetStockReportFile(IGetStockRemainingQuantity getStockRemainingQuantity, ICSVFileWriter csvFileWriter)
        {
            _getStockRemainingQuantity = getStockRemainingQuantity;
            _csvFileWriter = csvFileWriter;
        }

        public byte[] GetFile(IEnumerable<ProductVariant> variants)
        {
            return _csvFileWriter.GetFile(variants.Where(variant => variant.Product.Published),
                new Dictionary<string, Func<ProductVariant, object>>
                {
                    {"Name", variant => variant.FullName},
                    {"SKU", variant => variant.SKU},
                    {"Stock Remaining", variant => _getStockRemainingQuantity.Get(variant)}
                });
        }
    }
}