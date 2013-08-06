using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.Rules;
using MrCMS.Website;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.BulkStockUpdate
{
    public class BulkStockUpdateValidationServiceTests : InMemoryDatabaseTest
    {
        private IBulkStockUpdateValidationService _bulkStockUpdateValidationService;

        public BulkStockUpdateValidationServiceTests()
        {
            _bulkStockUpdateValidationService = new BulkStockUpdateValidationService();
        }

        [Fact]
        public void BulkStockUpdateValidationService_ValidateBusinessLogic_CallsAllIoCRegisteredRulesOnAllProductVariants()
        {
            var mockingKernel = new MockingKernel();
            var validationRules =
                Enumerable.Range(1, 10).Select(i => A.Fake<IBulkStockUpdateValidationRule>()).ToList();
            validationRules.ForEach(rule => mockingKernel.Bind<IBulkStockUpdateValidationRule>()
                                                                      .ToMethod(context => rule));
            MrCMSApplication.OverrideKernel(mockingKernel);

            var items = Enumerable.Range(1, 10).Select(i => new BulkStockUpdateDataTransferObject()).ToList();

            _bulkStockUpdateValidationService.ValidateBusinessLogic(items);

            validationRules.ForEach(
                rule =>
                EnumerableHelper.ForEach(items, product => A.CallTo(() => rule.GetErrors(product)).MustHaveHappened()));
        }

        //[Fact]
        //public void BulkStockUpdateValidationService_ValidateAndBulkStockUpdateProductVariants_ShouldReturnListOfProductVariantsAndNoErrors()
        //{
        //    var parseErrors = new Dictionary<string, List<string>>();
        //    var items = _bulkStockUpdateValidationService.ValidateAndBulkStockUpdateProductVariants(GetFile(), ref parseErrors);

        //    items.Count.ShouldBeEquivalentTo(1);
        //    parseErrors.Count.ShouldBeEquivalentTo(0);
        //}

        //[Fact]
        //public void BulkStockUpdateValidationService_ValidateAndBulkStockUpdateProductVariants_ShouldReturnProductVariantWithPrimaryPropertiesSet()
        //{
        //    var parseErrors = new Dictionary<string, List<string>>();
        //    var items = _bulkStockUpdateValidationService.ValidateAndBulkStockUpdateProductVariants(GetFile(), ref parseErrors);

        //    items.First().Name.ShouldBeEquivalentTo("TP");
        //    items.First().SKU.ShouldBeEquivalentTo("123");
        //    items.First().StockRemaining.ShouldBeEquivalentTo(22);
        //}

        //private static Stream GetFile()
        //{
        //    var items = new List<ProductVariant>()
        //        {
        //            new ProductVariant()
        //                {
        //                    Name = "TP",
        //                    SKU="123",
        //                    StockRemaining = 22
        //                }
        //        };

        //    var ms = new MemoryStream();
        //    var sw = new StreamWriter(ms);
        //    var w = new CsvWriter(sw);
            
        //    w.WriteField("Name");
        //    w.WriteField("SKU");
        //    w.WriteField("Stock Remaining");
        //    w.NextRecord();

        //    foreach (var item in items)
        //    {
        //        w.WriteField(item.Name);
        //        w.WriteField(item.SKU);
        //        w.WriteField(item.StockRemaining);
        //        w.NextRecord();
        //    }
        //    ms.Position = 0;
        //    sw.Flush();
        //    sw.Close();
        //    return ms;
        //}
    }
}