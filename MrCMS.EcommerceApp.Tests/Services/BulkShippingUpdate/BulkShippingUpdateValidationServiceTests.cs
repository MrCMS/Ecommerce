using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.Rules;
using MrCMS.Website;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services.BulkShippingUpdate
{
    public class BulkShippingUpdateValidationServiceTests : InMemoryDatabaseTest
    {
        private readonly IBulkShippingUpdateValidationService _bulkShippingUpdateValidationService;

        public BulkShippingUpdateValidationServiceTests()
        {
            _bulkShippingUpdateValidationService = new BulkShippingUpdateValidationService();
        }

        [Fact]
        public void BulkShippingUpdateValidationService_ValidateBusinessLogic_CallsAllIoCRegisteredRulesOnAllOrders()
        {
            var mockingKernel = new MockingKernel();
            var validationRules =
                Enumerable.Range(1, 10).Select(i => A.Fake<IBulkShippingUpdateValidationRule>()).ToList();
            validationRules.ForEach(rule => mockingKernel.Bind<IBulkShippingUpdateValidationRule>()
                                                                      .ToMethod(context => rule));
            MrCMSApplication.OverrideKernel(mockingKernel);

            var items = Enumerable.Range(1, 10).Select(i => new BulkShippingUpdateDataTransferObject()).ToList();

            _bulkShippingUpdateValidationService.ValidateBusinessLogic(items);

            validationRules.ForEach(
                rule =>
                EnumerableHelper.ForEach(items, product => A.CallTo(() => rule.GetErrors(product)).MustHaveHappened()));
        }

        [Fact]
        public void BulkShippingUpdateValidationService_ValidateAndBulkShippingUpdateOrders_ShouldReturnListOfOrdersAndNoErrors()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            var items = _bulkShippingUpdateValidationService.ValidateAndBulkShippingUpdateOrders(new MemoryStream(GetFile()), ref parseErrors);

            items.Count.ShouldBeEquivalentTo(1);
            parseErrors.Count.ShouldBeEquivalentTo(0);
        }

        [Fact]
        public void BulkShippingUpdateValidationService_ValidateAndBulkShippingUpdateOrders_ShouldReturnOrderWithPrimaryPropertiesSet()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            var items = _bulkShippingUpdateValidationService.ValidateAndBulkShippingUpdateOrders(new MemoryStream(GetFile()), ref parseErrors);

            items.First().OrderId.ShouldBeEquivalentTo(1);
            items.First().ShippingMethod.ShouldBeEquivalentTo("Test");
        }

        private static byte[] GetFile()
        {
            var items = new List<Order>()
                {
                    new Order()
                        {
                            Id = 1,
                            ShippingMethodName= "Test",
                        }
                };

            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var w = new CsvWriter(sw))
            {
                w.WriteField("Order Id");
                w.WriteField("Shipping Method");
                w.WriteField("Tracking Number");
                w.NextRecord();

                foreach (var item in items)
                {
                    w.WriteField(item.Id);
                    w.WriteField(item.ShippingMethodName);
                    w.WriteField("Courier");
                    w.NextRecord();
                }

                sw.Flush();
                var file = ms.ToArray();
                sw.Close();

                return file;
            }
        }
    }
}