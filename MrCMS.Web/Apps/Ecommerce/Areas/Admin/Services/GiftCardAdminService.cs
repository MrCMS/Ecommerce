using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GiftCardAdminService : IGiftCardAdminService
    {
        private readonly IGenerateGiftCardCode _generateGiftCardCode;
        private readonly IGetGiftCardTypeOptions _getGiftCardTypeOptions;
        private readonly ISession _session;

        public GiftCardAdminService(ISession session, IGenerateGiftCardCode generateGiftCardCode,
            IGetGiftCardTypeOptions getGiftCardTypeOptions)
        {
            _session = session;
            _generateGiftCardCode = generateGiftCardCode;
            _getGiftCardTypeOptions = getGiftCardTypeOptions;
        }

        public IPagedList<GiftCard> Search(GiftCardSearchQuery query)
        {
            IQueryOver<GiftCard, GiftCard> queryOver = _session.QueryOver<GiftCard>();
            if (!string.IsNullOrWhiteSpace(query.Recipient))
            {
                queryOver.Where(
                    x =>
                        x.RecipientEmail.IsInsensitiveLike(query.Recipient, MatchMode.Anywhere) ||
                        x.RecipientName.IsInsensitiveLike(query.Recipient, MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(query.Sender))
            {
                queryOver.Where(
                    x =>
                        x.SenderEmail.IsInsensitiveLike(query.Sender, MatchMode.Anywhere) ||
                        x.SenderName.IsInsensitiveLike(query.Sender, MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(query.GiftCode))
            {
                queryOver.Where(x => x.Code == query.GiftCode);
            }
            return queryOver.OrderBy(card => card.CreatedOn).Desc.Paged(query.Page);
        }

        public List<SelectListItem> GetTypeOptions()
        {
            return _getGiftCardTypeOptions.Get();
        }

        public List<SelectListItem> GetStatusOptions()
        {
            return Enum.GetValues(typeof(GiftCardStatus)).Cast<GiftCardStatus>()
                .BuildSelectItemList(status => status.ToString().BreakUpString(),
                    status => status.ToString(),
                    emptyItem: null);
        }

        public void Add(GiftCard giftCard)
        {
            _session.Transact(session => session.Save(giftCard));
        }

        public void Update(GiftCard giftCard)
        {
            _session.Transact(session => session.Update(giftCard));
        }

        public void Delete(GiftCard giftCard)
        {
            _session.Transact(session => session.Delete(giftCard));
        }

        public string GenerateCode()
        {
            return _generateGiftCardCode.Generate();
        }
    }
}