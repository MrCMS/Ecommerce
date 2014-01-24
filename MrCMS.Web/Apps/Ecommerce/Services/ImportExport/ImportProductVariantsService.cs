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
                var productVariant = _allVariants.SingleOrDefault(x => x.SKU == item.SKU) ?? new ProductVariant();

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

                if (!product.Variants.Contains(productVariant))
                {
                    product.Variants.Add(productVariant);
                }


                for (var i = productVariant.OptionValues.Count - 1; i >= 0; i--)
                {
                    var value = productVariant.OptionValues[i];
                    productVariant.OptionValues.Remove(value);
                    _productOptionManager.DeleteProductAttributeValue(value);
                }

                //Price Breaks
                _importProductVariantPriceBreaksService.ImportVariantPriceBreaks(item, productVariant);

                //Specifications
                _importProductOptionsService.ImportVariantSpecifications(item, product, productVariant);
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