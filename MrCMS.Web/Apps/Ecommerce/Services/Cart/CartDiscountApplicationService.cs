using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartDiscountApplicationService : ICartDiscountApplicationService
    {
        private readonly IKernel _kernel;
        private readonly ISession _session;
        private static readonly Dictionary<string, Type> LimitationCheckerTypes = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> ApplicationApplierTypes = new Dictionary<string, Type>();

        static CartDiscountApplicationService()
        {
            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<DiscountLimitation>()
                .Where(type => !type.ContainsGenericParameters))
            {
                var thisType = type;
                bool processed = false;
                while (!processed && thisType != null && thisType != typeof(DiscountLimitation))
                {
                    var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(DiscountLimitationChecker<>).MakeGenericType(thisType));
                    if (types.Any())
                    {
                        LimitationCheckerTypes.Add(type.FullName, types.First());
                        processed = true;
                    }
                    thisType = thisType.BaseType;
                }
            }
            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<DiscountApplication>()
                .Where(type => !type.ContainsGenericParameters))
            {
                var thisType = type;
                bool processed = false;
                while (!processed && thisType != null && thisType != typeof(DiscountApplication))
                {
                    var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(DiscountApplicationApplier<>).MakeGenericType(thisType));
                    if (types.Any())
                    {
                        ApplicationApplierTypes.Add(type.FullName, types.First());
                        processed = true;
                    }
                    thisType = thisType.BaseType;
                }
            }
        }

        public CartDiscountApplicationService(IKernel kernel, ISession session)
        {
            _kernel = kernel;
            _session = session;
        }

        public CheckLimitationsResult CheckLimitations(Discount discount, CartModel cart, IList<Discount> allDiscounts)
        {
            var limitations = _session.QueryOver<DiscountLimitation>()
                .Where(limitation => limitation.Discount.Id == discount.Id)
                .Cacheable()
                .List();

            var results = new CheckLimitationsResult[limitations.Count];
            for (var i = 0; i < limitations.Count; i++)
            {
                var limitation = limitations[i];
                var fullName = limitation.GetType().FullName;
                if (LimitationCheckerTypes.ContainsKey(fullName))
                {
                    var checker = _kernel.Get(LimitationCheckerTypes[fullName]) as DiscountLimitationChecker;
                    if (checker != null)
                    {
                        results[i] = checker.CheckLimitations(limitation, cart, allDiscounts);
                        continue;
                    }
                }
                results[i] = CheckLimitationsResult.CurrentlyInvalid("Limitation cannot be checked");
            }
            return CheckLimitationsResult.Combine(results);
        }

        public DiscountApplicationInfo ApplyDiscount(DiscountInfo discountInfo, CartModel cart)
        {
            var applications = _session.QueryOver<DiscountApplication>()
                .Where(application => application.Discount.Id == discountInfo.Discount.Id)
                .Cacheable()
                .List();
            var discountApplicationInfo = new DiscountApplicationInfo();
            foreach (var application in applications)
            {
                var fullName = application.GetType().FullName;
                if (!ApplicationApplierTypes.ContainsKey(fullName))
                    continue;

                var applier = _kernel.Get(ApplicationApplierTypes[fullName]) as DiscountApplicationApplier;
                if (applier != null)
                    discountApplicationInfo.Add(applier.Apply(application, cart, discountInfo.CheckLimitationsResult));
            }
            return discountApplicationInfo;
        }

    }
}