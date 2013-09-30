using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductOptionManagementService
    {
        List<SelectListItem> GetProductAttributeOptions(Product product);
        ProductOption Add(ProductOption productOption);
        void AddOption(Product product, ProductOption productOption);
        void RemoveOption(Product product, ProductOption productOption);
    }

    public class ProductOptionManagementService : IProductOptionManagementService
    {
        private readonly ISession _session;

        public ProductOptionManagementService(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetProductAttributeOptions(Product product)
        {
            var productAttributeOptions =
                _session.QueryOver<ProductOption>()
                        .OrderBy(option => option.Name)
                        .Asc.Cacheable()
                        .List().ToList();

            productAttributeOptions = productAttributeOptions.FindAll(option => !product.Options.Contains(option));

            var options = productAttributeOptions.BuildSelectItemList(option => option.Name,
                                                                      option => option.Id.ToString(),
                                                                      emptyItemText: "Select an option");
            options.Add(new SelectListItem { Text = "Add new", Value = "-1" });
            return options;
        }

        public ProductOption Add(ProductOption productOption)
        {
            _session.Transact(session => session.Save(productOption));
            return productOption;
        }

        public void AddOption(Product product, ProductOption productOption)
        {
            if (productOption != null && product != null)
            {
                product.Options.Add(productOption);
                productOption.Products.Add(product);
                foreach (var variant in product.Variants)
                {
                    variant.OptionValues.Add(new ProductOptionValue
                        {
                            ProductOption = productOption,
                            ProductVariant = variant,
                            Value = string.Empty,
                        });
                }
                _session.Transact(session =>
                    {
                        session.Update(product);
                        session.Update(productOption);
                    });
            }
        }

        public void RemoveOption(Product product, ProductOption productOption)
        {
            if (productOption != null && product != null)
            {
                product.Options.Remove(productOption);
                productOption.Products.Remove(product);
                _session.Transact(session =>
                    {
                        foreach (var variant in product.Variants.ToList())
                        {
                            foreach (var productOptionValue in variant.OptionValues.Where(value => value.ProductOption == productOption).ToList())
                            {
                                variant.OptionValues.Remove(productOptionValue);
                                session.Delete(productOptionValue);
                            }
                        }
                        session.Update(product);
                        session.Update(productOption);
                    });
            }
        }
    }
}