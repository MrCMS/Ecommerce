using System;
using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Web.Apps.Ecommerce.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Metadata
{
    public class ProductContainerMetadataTests
    {
        [Fact]
        public void ProductContainerMetaData_ChildrenListType_ShouldBeWhiteList()
        {
            var metadata = GetMetadata();

            metadata.ChildrenListType.Should().Be(ChildrenListType.WhiteList);
        }

        [Fact]
        public void ProductContainerMetaData_ChildrenList_ShouldBeJustProducts()
        {
            var metadata = GetMetadata();

            metadata.ChildrenList.Should().BeEquivalentTo(new List<Type> { typeof(Product) });
        }

        [Fact]
        public void ProductContainerMetaData_ShowChildrenInAdminNav_ShouldBeTrue()
        {
            var metadata = GetMetadata();

            metadata.ShowChildrenInAdminNav.Should().BeTrue();
        }
        
        private static ProductContainerMetadata GetMetadata()
        {
            return new ProductContainerMetadata();
        }
    }
}