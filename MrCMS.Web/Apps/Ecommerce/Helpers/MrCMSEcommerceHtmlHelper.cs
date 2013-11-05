using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class MrCMSEcommerceHtmlHelper
    {
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.SetInnerText(labelText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString GetSalesChannelInfo(this HtmlHelper<Order> htmlHelper)
        {
            var order = htmlHelper.ViewData.Model;
            if (order == null)
                return MvcHtmlString.Empty;
            if (EcommerceApp.SalesChannelApps.ContainsKey(order.SalesChannel))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = EcommerceApp.SalesChannelApps[order.SalesChannel];

            ViewEngineResult viewEngineResult =
                ViewEngines.Engines.FindView(new ControllerContext(htmlHelper.ViewContext.RequestContext, htmlHelper.ViewContext.Controller), order.SalesChannel, "");
            if (viewEngineResult.View != null)
            {
                try
                {
                    return htmlHelper.Partial(order.SalesChannel, order);
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                }
            }
            return MvcHtmlString.Empty;
        }
    }
}