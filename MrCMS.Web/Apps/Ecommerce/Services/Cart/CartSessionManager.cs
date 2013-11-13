using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartSessionManager : ICartSessionManager
    {
        private readonly ISession _session;
        private readonly Site _site;
        private const string _passPhrase = "MrCMS Ecommerce's passphrase for session encryption and decryption";

        public CartSessionManager(ISession session,  Site site)
        {
            _session = session;
            _site = site;
        }

        public T GetSessionValue<T>(string key, Guid userGuid, T defaultValue = default(T), bool encrypted = false)
        {
            var queryOver = _session.QueryOver<SessionData>().Where(data => data.UserGuid == userGuid && data.Site.Id == _site.Id && data.Key == key);

            if (encrypted)
                queryOver = queryOver.Where(data => data.ExpireOn >= CurrentRequestData.Now);

            var sessionData =
                queryOver
                        .Take(1)
                        .Cacheable()
                        .SingleOrDefault();
            if (sessionData == null)
                return defaultValue;
            try
            {
                var data = sessionData.Data;
                if (encrypted)
                    data = StringCipher.Decrypt(data, _passPhrase);
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void SetSessionValue<T>(string key, Guid userGuid, T item, bool encrypt = false)
        {
            var sessionData =
                _session.QueryOver<SessionData>()
                        .Where(data => data.UserGuid == userGuid && data.Site.Id == _site.Id && data.Key == key)
                        .Take(1)
                        .Cacheable()
                        .SingleOrDefault() ?? new SessionData { Key = key, UserGuid = userGuid };


            var obj = JsonConvert.SerializeObject(item);
            if (encrypt)
            {
                obj = StringCipher.Encrypt(obj, _passPhrase);
                sessionData.ExpireOn = CurrentRequestData.Now.AddMinutes(5);
            }
            sessionData.Data = obj;
            _session.Transact(session => session.SaveOrUpdate(sessionData));
        }

        public void RemoveValue(string key, Guid userGuid)
        {
            var sessionData =
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
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
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
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }
}