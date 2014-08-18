using System;
using System.Linq;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderLineCreator : IOrderLineCreator
    {
        private readonly IFileService _fileService;

        public OrderLineCreator(IFileService fileService)
        {
            _fileService = fileService;
        }

        public OrderLine GetOrderLine(CartItem item)
        {
            var options = string.Join(", ", item.Item.OptionValues.Select(value => value.FormattedValue));

            var orderLine = new OrderLine
            {
                UnitPrice = item.UnitPrice,
                UnitPricePreTax = item.UnitPricePreTax,
                Weight = item.Weight,
                TaxRate = item.TaxRatePercentage,
                Tax = item.Tax,
                Quantity = item.Quantity,
                ProductVariant = item.Item,
                PricePreTax = item.PricePreTax,
                Price = item.Price,
                SKU = item.Item.SKU,
                Name = item.Item.FullName,
                Options = options,
                Discount = item.DiscountAmount,
                RequiresShipping = item.RequiresShipping,
                Data = item.Data
            };
            if (item.IsDownloadable)
            {
                orderLine.IsDownloadable = true;
                orderLine.AllowedNumberOfDownloads = item.AllowedNumberOfDownloads;
                orderLine.DownloadExpiresOn =
                    (item.AllowedNumberOfDaysForDownload.HasValue && item.AllowedNumberOfDaysForDownload > 0)
                        ? CurrentRequestData.Now.AddDays(
                            item.AllowedNumberOfDaysForDownload
                                .GetValueOrDefault())
                        : (DateTime?)null;
                orderLine.NumberOfDownloads = 0;
                var fileByUrl = _fileService.GetFileByUrl(item.DownloadFileUrl);
                if (fileByUrl != null)
                {
                    orderLine.DownloadFileUrl = fileByUrl.FileUrl;
                    orderLine.DownloadFileContentType = fileByUrl.ContentType;
                    orderLine.DownloadFileName = fileByUrl.FileName;
                }
                else
                {
                    orderLine.DownloadFileUrl = item.DownloadFileUrl;
                }
            }
            return orderLine;
        }
    }
}