using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class GeneralHelper
    {
        public static string GetDescriptionFromEnum(Enum value)
        {
            var attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public static bool IsValidInput<T>(this string value) where T : struct
        {
             try
             {
                 if (value.HasValue())
                 {
                     var convertedValue=Convert.ChangeType(value, typeof(T));
                 }
                 return true;
             }
             catch (Exception)
             {
                 return false;
             }
        }
        public static T GetValue<T>(string value) where T : struct
        {
            try
            {
                if (value.HasValue())
                {
                    var convertedValue = Convert.ChangeType(value, typeof(T));
                    return (T)convertedValue;
                }
                return default(T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        public static bool IsValidDateTime(this string value)
        {
            try
            {
                DateTime result;
                DateTime.TryParse(value, out result);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static IEnumerable<SelectListItem> SelectDefaultValue(IEnumerable<SelectListItem> list, string value)
        {
            if (list != null && list.Any() && !String.IsNullOrWhiteSpace(value))
            {
                foreach (var selectListItem in list)
                    selectListItem.Selected = false;
                if (list.SingleOrDefault(x => x.Value == value) != null)
                    list.SingleOrDefault(x => x.Value == value).Selected = true;
            }
            return list;
        }

        public static IPagedList<T> Paged<T>(this IEnumerable<T> items, int pageNumber, int pageSize)
             where T : class
        {
            IEnumerable<T> values = items
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList<T>();

            var rowCount = items.Count();

            return new StaticPagedList<T>(values, pageNumber, pageSize, rowCount);
        }

        public static List<SelectListItem> GetEnumOptionsWithEmpty<T>() where T : struct
        {
            return
                Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .BuildSelectItemList(item => GetDescriptionFromEnum(item as Enum), item => item.ToString(), emptyItemText: "Please select...");
        }

        public static string GetValidImageUrl(string imageUrl)
        {
            if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                return imageUrl.Replace("https", "http");

            var baseUrl = "http://" + CurrentRequestData.CurrentSite.BaseUrl;
            if (CurrentRequestData.CurrentSite.BaseUrl.Contains("http://") || CurrentRequestData.CurrentSite.BaseUrl.Contains("https://"))
                baseUrl = CurrentRequestData.CurrentSite.BaseUrl.Replace("https", "http");
            return baseUrl + imageUrl;
        }

        public static string GetValidProductVariantUrl(ProductVariant productVariant)
        {
            var url = productVariant.Product.UrlSegment;
            if (url.Contains("http://") || url.Contains("https://"))
                return url.Replace("https", "http");

            var baseUrl = "http://" + CurrentRequestData.CurrentSite.BaseUrl;
            if (CurrentRequestData.CurrentSite.BaseUrl.Contains("http://") || CurrentRequestData.CurrentSite.BaseUrl.Contains("https://"))
                baseUrl = CurrentRequestData.CurrentSite.BaseUrl;
            return string.Format("{0}/{1}?variant={2}", baseUrl, url, productVariant.Id);
        }
    }
}