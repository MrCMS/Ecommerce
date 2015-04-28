using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.TemplateData;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Services.Parsing;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class ProductListItemParser : INewsletterItemParser<ProductList>
    {
        private static readonly Regex RowRegex = new Regex(@"\[(?i)ProductRow\]");
        private static readonly Regex ProductRegex = new Regex(@"\[(?i)Product\]");
        private readonly INewsletterProductParser _productParser;
        private readonly IGetContentItemTemplateData _getContentItemTemplateData;
        private readonly ISession _session;

        public ProductListItemParser(ISession session, INewsletterProductParser productParser,
            IGetContentItemTemplateData getContentItemTemplateData)
        {
            _session = session;
            _productParser = productParser;
            _getContentItemTemplateData = getContentItemTemplateData;
        }

        public string Parse(NewsletterTemplate template, ProductList item)
        {
            var templateData = _getContentItemTemplateData.Get<ProductListTemplateData>(template);
            if (templateData == null)
                return string.Empty;

            string output = templateData.ProductGridTemplate;
            if (string.IsNullOrWhiteSpace(output))
                return string.Empty;

            Product[] products = GetProducts(item);
            output = RowRegex.Replace(output, match => ParseRows(templateData, products));
            return output;
        }

        private Product[] GetProducts(ProductList productList)
        {
            List<int> productIds = productList.Products
                .Split(',')
                .Select(productToken => productToken.Split('/'))
                .Select(splitToken =>
                {
                    int id;
                    Int32.TryParse(splitToken.FirstOrDefault(), out id);
                    return id;
                })
                .ToList();

            return _session.QueryOver<Product>()
                .WhereRestrictionOn(product => product.Id).IsIn(productIds)
                .List()
                .ToArray();
        }

        private string ParseRows(ProductListTemplateData templateData, Product[] products)
        {
            int numProductsInRow = ProductRegex.Matches(templateData.ProductRowTemplate).Count;
            string output = string.Empty;

            for (int i = 0; i < products.Length; i += numProductsInRow)
            {
                output += ParseRow(templateData, products.Skip(i).Take(numProductsInRow));
            }

            return output;
        }

        private string ParseRow(ProductListTemplateData templateData, IEnumerable<Product> rowProducts)
        {
            string rowOutput = rowProducts.Aggregate(templateData.ProductRowTemplate,
                (current, product) =>
                    ProductRegex.Replace(current, replace => _productParser.Parse(templateData, product), 1));

            // Blank any remaining shortcodes as we may have ran out of products
            rowOutput = ProductRegex.Replace(rowOutput, string.Empty);

            return rowOutput;
        }
    }
}