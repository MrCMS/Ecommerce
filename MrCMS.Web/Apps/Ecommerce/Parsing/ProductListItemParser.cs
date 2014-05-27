using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class ProductListItemParser : INewsletterItemParser<ProductList>
    {
        private static readonly Regex RowRegex = new Regex(@"\[(?i)ProductRow\]");
        private static readonly Regex ProductRegex = new Regex(@"\[(?i)Product\]");
        private readonly INewsletterItemParser<Product> _productParser;
        private readonly ISession _session;

        public ProductListItemParser(ISession session, INewsletterItemParser<Product> productParser)
        {
            _session = session;
            _productParser = productParser;
        }

        public string Parse(NewsletterTemplate template, ProductList item)
        {
            string output = template.ProductGridTemplate;
            Product[] products = GetProducts(item);
            output = RowRegex.Replace(output, match => ParseRows(template, products));
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

        private string ParseRows(NewsletterTemplate template, Product[] products)
        {
            int numProductsInRow = ProductRegex.Matches(template.ProductRowTemplate).Count;
            string output = string.Empty;

            for (int i = 0; i < products.Length; i += numProductsInRow)
            {
                output += ParseRow(template, products.Skip(i).Take(numProductsInRow));
            }

            return output;
        }

        private string ParseRow(NewsletterTemplate template, IEnumerable<Product> rowProducts)
        {
            string rowOutput = template.ProductRowTemplate;

            foreach (Product product in rowProducts)
            {
                rowOutput = ProductRegex.Replace(rowOutput, replace => _productParser.Parse(template, product), 1);
            }

            // Blank any remaining shortcodes as we may have ran out of products
            rowOutput = ProductRegex.Replace(rowOutput, string.Empty);

            return rowOutput;
        }
    }
}