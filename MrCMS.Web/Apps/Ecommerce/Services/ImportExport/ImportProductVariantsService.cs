using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductVariantsService : IImportProductVariantsService
    {
        private readonly IImportProductVariantPriceBreaksService _importProductVariantPriceBreaksService;
        private readonly IImportProductOptionsService _importProductOptionsService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IProductOptionManager _productOptionManager;
        private readonly ISession _session;

        private HashSet<ProductVariant> _allVariants;

        public ImportProductVariantsService(IImportProductVariantPriceBreaksService importPriceBreaksService, IImportProductOptionsService importProductOptionsService,
             ITaxRateManager taxRateManager, IProductOptionManager productOptionManager, ISession session)
        {
            _taxRateManager = taxRateManager;
            _productOptionManager = productOptionManager;
            _session = session;
            _importProductVariantPriceBreaksService = importPriceBreaksService;
            _importProductOptionsService = importProductOptionsService;
        }

        public IImportProductVariantsService Initialize()
        {
            _allVariants = new HashSet<ProductVariant>(_session.QueryOver<ProductVariant>().List());
            _importProductOptionsService.Initialize();
            return this;
        }

        public IEnumerable<ProductVariant> ImportVariants(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            foreach (var item in dataTransferObject.ProductVariants)
            {
                ProductVariant productVariant;
                if (_allVariants.SingleOrDefault(x => x.SKU == item.SKU) != null)
                    productVariant = _allVariants.SingleOrDefault(x => x.SKU == item.SKU);
                else
                {
                    productVariant = new ProductVariant();
                    _allVariants.Add(productVariant);
                }

                productVariant.Name = item.Name;
                productVariant.SKU = item.SKU;
                productVariant.Barcode = item.Barcode;
                productVariant.ManufacturerPartNumber = item.ManufacturerPartNumber;
                productVariant.BasePrice = item.Price;
                productVariant.PreviousPrice = item.PreviousPrice;
                productVariant.StockRemaining = item.Stock.HasValue ? item.Stock.Value : 0;
                productVariant.Weight = item.Weight.HasValue ? item.Weight.Value : 0;
                productVariant.TrackingPolicy = item.TrackingPolicy;
                productVariant.TaxRate = (item.TaxRate.HasValue && item.TaxRate.Value != 0) ? _taxRateManager.Get(item.TaxRate.Value) : _taxRateManager.GetDefaultRate();
                productVariant.Product = product;

                //if (!product.Variants.Contains(productVariant))
                //{
                //    product.Variants.Add(productVariant);
                //}


                var optionsToAdd =
                    item.Options.Where(
                        s =>
                        !productVariant.OptionValues.Select(value => value.ProductOption.Name)
                                        .Contains(s.Key, StringComparer.OrdinalIgnoreCase)).ToList();
                var optionsToRemove =
                    productVariant.OptionValues.Where(
                        value => !item.Options.Keys.Contains(value.ProductOption.Name, StringComparer.OrdinalIgnoreCase))
                                  .ToList();
                var optionsToUpdate =
                    productVariant.OptionValues.Where(value => !optionsToRemove.Contains(value)).ToList();

                foreach (var option in optionsToAdd)
                {
                    var productOption = product.Options.FirstOrDefault(po => po.Name == option.Key);
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
                foreach (var value in optionsToRemove)
                {
                    var productOption = value.ProductOption;
                    productVariant.OptionValues.Remove(value);
                    productOption.Values.Remove(value);
                    _session.Delete(value);
                }
                foreach (var value in optionsToUpdate)
                {
                    var key =
                        item.Options.Keys.FirstOrDefault(
                            s => s.Equals(value.ProductOption.Name, StringComparison.OrdinalIgnoreCase));
                    if (key != null) value.Value = item.Options[key];
                }

                //Price Breaks
                _importProductVariantPriceBreaksService.ImportVariantPriceBreaks(item, productVariant);

                _session.SaveOrUpdate(productVariant);
            }
            var variantsToRemove =
                product.Variants.Where(
                    variant => !dataTransferObject.ProductVariants.Select(o => o.SKU).Contains(variant.SKU)).ToList();
            foreach (var variant in variantsToRemove)
            {
                _allVariants.Remove(variant);
                product.Variants.Remove(variant);
                _session.Delete(variant);
            }

            return dataTransferObject.ProductVariants.Any() ? product.Variants : null;
        }
    }

    public interface IImportProductOptionsService
    {
        IImportProductOptionsService Initialize();
        void ImportVariantSpecifications(ProductVariantImportDataTransferObject dataTransferObject, Product product,
                                         ProductVariant productVariant);
    }

    public class ImportProductOptionsService : IImportProductOptionsService
    {
        private readonly ISession _session;
        private HashSet<ProductOption> _productOptions;

        public ImportProductOptionsService(ISession session)
        {
            _session = session;
        }


        public IImportProductOptionsService Initialize()
        {
            _productOptions = new HashSet<ProductOption>(_session.QueryOver<ProductOption>().List());
            return this;
        }

        public void ImportVariantSpecifications(ProductVariantImportDataTransferObject item, Product product, ProductVariant productVariant)
        {
            productVariant.OptionValues.Clear();
            foreach (var opt in item.Options)
            {
                var option =
                    _productOptions.FirstOrDefault(
                        productOption => productOption.Name.Equals(opt.Key, StringComparison.InvariantCultureIgnoreCase)) ??
                    new ProductOption { Name = opt.Key };
                if (!product.Options.Contains(option))
                    product.Options.Add(option);

                var productOptionValue = productVariant.OptionValues.FirstOrDefault(x => x.ProductOption.Id == option.Id);
                if (productOptionValue == null)
                {
                    productVariant.OptionValues.Add(new ProductOptionValue
                    {
                        ProductOption = option,
                        ProductVariant = productVariant,
                        Value = opt.Value
                    });
                }
                else
                {
                    productOptionValue.Value = opt.Value;
                }
            }
        }
    }
}