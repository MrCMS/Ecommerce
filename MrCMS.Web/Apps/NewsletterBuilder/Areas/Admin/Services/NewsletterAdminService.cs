using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using NHibernate;
using NHibernate.Linq;
using Ninject;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services
{
    public class NewsletterAdminService : INewsletterAdminService
    {
        private readonly ISession _session;
        private readonly ICloneNewsletterService _cloneNewsletterService;

        public NewsletterAdminService(ISession session, ICloneNewsletterService cloneNewsletterService)
        {
            _session = session;
            _cloneNewsletterService = cloneNewsletterService;
        }

        public IList<Newsletter> GetAll()
        {
            return _session.QueryOver<Newsletter>().Cacheable().List();
        }

        public void Add(Newsletter newsletter)
        {
            _session.Transact(session => session.Save(newsletter));
        }

        public void Edit(Newsletter newsletter)
        {
            _session.Transact(session => session.Update(newsletter));
        }

        public void Delete(Newsletter newsletter)
        {
            _session.Transact(session => session.Delete(newsletter));
        }

        public void UpdateContentItemsDisplayOrder(List<SortItem> items)
        {
            _session.Transact(session => items.ForEach(item =>
            {
                var formItem = session.Get<ContentItem>(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }

        public List<SelectListItem> GetTemplateOptions()
        {
            return _session.Query<NewsletterTemplate>().Select(template => new {template.Name, template.Id})
                .BuildSelectItemList(x => x.Name, x => x.Id.ToString(), emptyItemText: "Select Template");
        }

        public HashSet<Type> GetContentItemTypes()
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<ContentItem>();
        }

        public Newsletter Clone(Newsletter newsletter)
        {
            return _cloneNewsletterService.Clone(newsletter);
        }
    }

    public interface ICloneNewsletterService
    {
        Newsletter Clone(Newsletter newsletter);
    }

    public class CloneNewsletterService : ICloneNewsletterService
    {
        private readonly ISession _session;
        private readonly IKernel _kernel;

        public CloneNewsletterService(ISession session, IKernel kernel)
        {
            _session = session;
            _kernel = kernel;
        }

        private static readonly Dictionary<Type, Type> CloneContentItemTypes;

        static CloneNewsletterService()
        {
            CloneContentItemTypes = new Dictionary<Type, Type>();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<ContentItem>().Where(type => !type.ContainsGenericParameters))
            {
                var cloneType =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(ContentItemCloner<>).MakeGenericType(type))
                        .FirstOrDefault();
                if (cloneType != null)
                    CloneContentItemTypes.Add(type, cloneType);
            }
        }

        public Newsletter Clone(Newsletter newsletter)
        {
            var clone = new Newsletter
            {
                NewsletterTemplate = newsletter.NewsletterTemplate,
                Name = newsletter.Name,
                ContentItems = new List<ContentItem>()
            };

            foreach (var item in newsletter.ContentItems.OrderBy(x => x.DisplayOrder))
            {
                var type = item.GetType();
                if (CloneContentItemTypes.ContainsKey(type))
                {
                    var cloneType = CloneContentItemTypes[type];
                    var cloner = _kernel.Get(cloneType) as CloneContentItemBase;
                    if (cloner != null)
                    {
                        var contentItem = cloner.Clone(item);
                        contentItem.DisplayOrder = item.DisplayOrder;
                        contentItem.Name = item.Name;
                        contentItem.Newsletter = clone;
                        clone.ContentItems.Add(contentItem);
                    }
                }
            }

            _session.Transact(session =>
            {
                session.Save(clone);
                foreach (var item in clone.ContentItems)
                {
                    session.Save(item);
                }
            });

            return clone;
        }
    }
}