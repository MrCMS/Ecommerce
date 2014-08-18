using System;
using System.Text;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GenerateGiftCardCode : IGenerateGiftCardCode
    {
        private const string availableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly ISession _session;

        public GenerateGiftCardCode(ISession session)
        {
            _session = session;
        }

        public string Generate()
        {
            var random = new Random();
            string code = GetCode(random);
            while (!IsUnique(code))
            {
                code = GetCode(random);
            }
            return code;
        }

        private bool IsUnique(string code)
        {
            return !_session.QueryOver<GiftCard>().Where(card => card.Code == code).Any();
        }

        private static string GetCode(Random random)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < 12; i++)
            {
                stringBuilder.Append(availableCharacters[random.Next(0, availableCharacters.Length)]);
            }
            return stringBuilder.ToString();
        }
    }
}