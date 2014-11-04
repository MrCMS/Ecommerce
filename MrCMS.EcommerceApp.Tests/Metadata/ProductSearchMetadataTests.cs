using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Web.Apps.Ecommerce.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;
using FluentAssertions;

namespace MrCMS.EcommerceApp.Tests.Metadata
{
    public class ProductSearchMetadataTests
    {
        private readonly ProductSearchMetadata _metadata;

        public ProductSearchMetadataTests()
        {
            _metadata = new ProductSearchMetadata();
        }
        [Fact]
        public void ProductSearchMetaData_ChildrenListType_ShouldBeWhiteList()
        {
            _metadata.ChildrenListType.Should().Be(ChildrenListType.WhiteList);
        }

        [Fact]
        public void ProductSearchMetaData_ChildrenList_ShouldBeJustCategories()
        {
            _metadata.ChildrenList.Should().BeEquivalentTo(new List<Type> {typeof (Category)});
        }

        [Fact]
        public void ProductSearchMetaData_ShowChildrenInAdminNav_ShouldBeFalse()
        {
            _metadata.ShowChildrenInAdminNav.Should().BeFalse();
        }
    }
}