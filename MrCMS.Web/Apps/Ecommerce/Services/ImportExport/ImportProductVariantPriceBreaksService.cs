using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductVariantPriceBreaksService : IImportProductVariantPriceBreaksService
    {
        private readonly ISession _session;

        public ImportProductVariantPriceBreaksService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<PriceBreak> ImportVariantPriceBreaks(ProductVariantImportDataTransferObject dto, ProductVariant productVariant)
        {

            var priceBreaksToAdd = dto.PriceBreaks.Where(s => !productVariant.PriceBreaks.Select(@break => @break.Quantity).Contains(s.Key)).ToList();
            var priceBreaksToRemove = productVariant.PriceBreaks.Where(@break => !dto.PriceBreaks.Keys.Contains(@break.Quantity)).ToList();
            var priceBreaksToUpdate = productVariant.PriceBreaks.Where(@break => !priceBreaksToRemove.Contains(@break)).ToList();
            foreach (var item in priceBreaksToAdd)
            {
                var priceBreak = new PriceBreak
                                     {
                                         Quantity = item.Key,
                                         Price = item.Value,
                                         ProductVariant = productVariant
                                     };
                productVariant.PriceBreaks.Add(priceBreak);
            }

            foreach (var priceBreak in priceBreaksToRemove)
            {
                productVariant.PriceBreaks.Remove(priceBreak);
                _session.Delete(priceBreak);
            }
            foreach (var priceBreak in priceBreaksToUpdate)
                priceBreak.Price = dto.PriceBreaks[priceBreak.Quantity];
            return productVariant.PriceBreaks;
        }
    }
}