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
        private HashSet<PriceBreak> _priceBreaks;

        public ImportProductVariantPriceBreaksService(ISession session)
        {
            _session = session;
        }

        public IImportProductVariantPriceBreaksService Initialize()
        {
            _priceBreaks = new HashSet<PriceBreak>(_session.QueryOver<PriceBreak>().List());
            return this;
        }

        public IEnumerable<PriceBreak> ImportVariantPriceBreaks(ProductVariantImportDataTransferObject item, ProductVariant productVariant)
        {
            productVariant.PriceBreaks.Clear();
            foreach (var priceBreakItem in item.PriceBreaks)
            {
                var priceBreak = _priceBreaks.SingleOrDefault(x => x.Quantity == priceBreakItem.Key && x.ProductVariant.Id == productVariant.Id);
                if (priceBreak == null)
                {
                    priceBreak = new PriceBreak
                        {
                            Price = priceBreakItem.Value,
                            Quantity = priceBreakItem.Key,
                            ProductVariant = productVariant
                        };
                    productVariant.PriceBreaks.Add(priceBreak);
                }
            }
            return productVariant.PriceBreaks;
        }
    }
}