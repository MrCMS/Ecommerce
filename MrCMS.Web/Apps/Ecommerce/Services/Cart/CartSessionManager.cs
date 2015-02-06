using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartSessionManager : ICartSessionManager
    {
        private readonly Dictionary<Guid, HashSet<SessionData>> _cache;
        private readonly ISession _session;
        private readonly Site _site;
        private readonly EcommerceSettings _ecommerceSettings;

        public CartSessionManager(ISession session, Site site, EcommerceSettings ecommerceSettings)
        {
            _session = session;
            _site = site;
            _ecommerceSettings = ecommerceSettings;
            _cache = new Dictionary<Guid, HashSet<SessionData>>();
        }

        public T GetSessionValue<T>(string key, Guid userGuid, T defaultValue = default(T), bool encrypted = false)
        {
            HashSet<SessionData> userData;
            if (_cache.ContainsKey(userGuid))
            {
                userData = _cache[userGuid];
            }
            else
            {
                userData = new HashSet<SessionData>(
                    _session.QueryOver<SessionData>().Where(data => data.UserGuid == userGuid).Cacheable().List());
                _cache[userGuid] = userData;
            }
            IEnumerable<SessionData> queryOver = userData.Where(data => (data.ExpireOn == null || data.ExpireOn >= CurrentRequestData.Now) && data.Key == key);

            SessionData sessionData = queryOver.FirstOrDefault();

            if (sessionData == null)
                return defaultValue;
            try
            {
                string data = sessionData.Data;
                if (encrypted)
                    data = StringCipher.Decrypt(data, _ecommerceSettings.EncryptionPassPhrase);
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void SetSessionValue<T>(string key, Guid userGuid, T item, TimeSpan? expireIn, bool encrypt = false)
        {
            var expiry = expireIn.GetValueOrDefault(TimeSpan.FromDays(_ecommerceSettings.DefaultSessionExpiryDays));
            if (_cache.ContainsKey(userGuid))
            {
                _cache.Remove(userGuid);
            }
            SessionData sessionData =
                _session.QueryOver<SessionData>()
                    .Where(data => data.UserGuid == userGuid && data.Site.Id == _site.Id && data.Key == key)
                    .Take(1)
                    .Cacheable()
                    .SingleOrDefault() ?? new SessionData { Key = key, UserGuid = userGuid };


            string obj = JsonConvert.SerializeObject(item);
            if (encrypt)
            {
                obj = StringCipher.Encrypt(obj, _ecommerceSettings.EncryptionPassPhrase);
            }

            sessionData.Data = obj;
            var now = CurrentRequestData.Now;
            sessionData.ExpireOn = now.Add(expiry);
            _session.Transact(session => session.SaveOrUpdate(sessionData));
        }

        public void RemoveValue(string key, Guid userGuid)
        {
            SessionData sessionData =
                _session.QueryOver<SessionData>()
                    .Where(data => data.UserGuid == userGuid && data.Site.Id == _site.Id && data.Key == key)
                    .Take(1)
                    .Cacheable()
                    .SingleOrDefault();
            if (sessionData != null)
                _session.Transact(session => session.Delete(sessionData));
        }
    }

    public static class StringCipher
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "ha78f2435l97asyx";


        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        public static string Encrypt(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            var password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }
}