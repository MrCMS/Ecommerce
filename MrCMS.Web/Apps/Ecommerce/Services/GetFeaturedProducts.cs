using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetFeaturedProducts : GetWidgetModelBase<FeaturedProducts>
    {
        private readonly ISession _session;
        private readonly CartModel _cart;

        public GetFeaturedProducts(ISession session, CartModel cart)
        {
            _session = session;
            _cart = cart;
        }

        public override object GetModel(FeaturedProducts widget)
        {
            var model = new FeaturedProductsViewModel { Title = widget.Name, Products = new List<Product>() };
            try
            {
                var rawValues = widget.ListOfFeaturedProducts.Split(',');
                var ids = new List<int>();
                foreach (var items in rawValues.Select(value => value.Split('/')))
                {
                    int id;
                    if (int.TryParse(items[0], out id) && id != 0)
                        ids.Add(id);
                }
                model.Products.AddRange(_session.QueryOver<Product>()
                    .Where(arg => arg.Id.IsIn(ids.ToList()))
                    .Cacheable()
                    .List()
                    .OrderBy(arg => ids.IndexOf(arg.Id)));
            }
            catch (Exception)
            {
                model.Title = "Error during widget loading. Review widget settings in Administration.";
            }
            model.Cart = _cart;
            return model;
        }
    }
}