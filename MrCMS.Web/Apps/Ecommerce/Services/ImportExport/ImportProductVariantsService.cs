using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.ETags;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductVariantsService : IImportProductVariantsService
    {
        private readonly IImportProductVariantPriceBreaksService _importProductVariantPriceBreaksService;
        private readonly ISession _session;
        private readonly IETagAdminService _eTagAdminService;
        private readonly ITaxRateManager _taxRateManager;

        public ImportProductVariantsService(IImportProductVariantPriceBreaksService importPriceBreaksService,
            ITaxRateManager taxRateManager, ISession session, IETagAdminService eTagAdminService)
        {
            _taxRateManager = taxRateManager;
            _session = session;
            _eTagAdminService = eTagAdminService;
            _importProductVariantPriceBreaksService = importPriceBreaksService;
        }

        public IEnumerable<ProductVariant> ImportVariants(ProductImportDataTransferObject dataTransferObject,
            Product product)
        {
            foreach (ProductVariantImportDataTransferObject item in dataTransferObject.ProductVariants)
            {
                ProductVariant productVariant =
                    _session.QueryOver<ProductVariant>()
                        .Where(variant => variant.SKU.IsInsensitiveLike(item.SKU, MatchMode.Exact))
                        .Take(1)
                        .SingleOrDefault();

                if (productVariant == null)
                {
                    productVariant = new ProductVariant();
                    _session.Transact(session => session.Save(productVariant));
                }

                if (!product.Variants.Contains(productVariant))
                    product.Variants.Add(productVariant);

                productVariant.Name = item.Name;
                productVariant.SKU = item.SKU;
                productVariant.Barcode = item.Barcode;
                productVariant.ManufacturerPartNumber = item.ManufacturerPartNumber;
                productVariant.BasePrice = item.Price;
                productVariant.PreviousPrice = item.PreviousPrice;
                productVariant.StockRemaining = item.Stock.HasValue ? item.Stock.Value : 0;
                productVariant.Weight = item.Weight.HasValue ? item.Weight.Value : 0;
                productVariant.TrackingPolicy = item.TrackingPolicy;
                productVariant.TaxRate = (item.TaxRate.HasValue && item.TaxRate.Value != 0)
                    ? _taxRateManager.Get(item.TaxRate.Value)
                    : _taxRateManager.GetDefaultRate();
                productVariant.Product = product;

                if (!string.IsNullOrEmpty(item.ETag))
                {
                    var eTag = _eTagAdminService.GetETagByName(item.ETag);
                    if (eTag == null)
                    {
                        eTag = new ETag
                        {
                            Name = item.ETag
                        };
                        _eTagAdminService.Add(eTag);
                    }

                    productVariant.ETag = eTag;
                }
                else
                    productVariant.ETag = null;

                List<KeyValuePair<string, string>> optionsToAdd =
                    item.Options.Where(
                        s =>
                            !productVariant.OptionValues.Select(value => value.ProductOption.Name)
                                .Contains(s.Key, StringComparer.OrdinalIgnoreCase)).ToList();
                List<ProductOptionValue> optionsToRemove =
                    productVariant.OptionValues.Where(
                        value => !item.Options.Keys.Contains(value.ProductOption.Name, StringComparer.OrdinalIgnoreCase))
                        .ToList();
                List<ProductOptionValue> optionsToUpdate =
                    productVariant.OptionValues.Where(value => !optionsToRemove.Contains(value)).ToList();

                foreach (var option in optionsToAdd)
                {
                    ProductOption productOption = product.Options.FirstOrDefault(po => po.Name == option.Key);
                    if (productOption != null)
                    {
                        var productOptionValue = new ProductOptionValue
                        {
                            ProductOption = productOption,
                            ProductVariant = productVariant,
                            Value = option.Value
                        };
                        productVariant.OptionValues.Add(productOptionValue);
                        productOption.Values.Add(productOptionValue);
                    }
                }
                foreach (ProductOptionValue value in optionsToRemove)
                {
                    ProductOption productOption = value.ProductOption;
                    productVariant.OptionValues.Remove(value);
                    productOption.Values.Remove(value);
                    ProductOptionValue closureValue = value;
                    _session.Transact(session => session.Delete(closureValue));
                }
                foreach (ProductOptionValue value in optionsToUpdate)
                {
                    string key =
                        item.Options.Keys.FirstOrDefault(
                            s => s.Equals(value.ProductOption.Name, StringComparison.OrdinalIgnoreCase));
                    if (key != null) value.Value = item.Options[key];
                }

                //Price Breaks
                _importProductVariantPriceBreaksService.ImportVariantPriceBreaks(item, productVariant);

                _session.SaveOrUpdate(productVariant);
            }
            List<ProductVariant> variantsToRemove =
                product.Variants.Where(
                    variant => !dataTransferObject.ProductVariants.Select(o => o.SKU).Contains(variant.SKU)).ToList();
            foreach (ProductVariant variant in variantsToRemove)
            {
                product.Variants.Remove(variant);
                ProductVariant closureVariant = variant;
                _session.Transact(session => session.Delete(closureVariant));
            }

            return dataTransferObject.ProductVariants.Any() ? product.Variants : null;
        }
    }
}