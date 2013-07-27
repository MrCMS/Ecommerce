using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Website;
using PayPal.Exception;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public static class PayPalHelper
    {
        public static T2 HandleResponse<T1, T2>(this T1 apiResponse, Action<T1, T2> onSuccess, Action<T1, T2> onFailure)
            where T2 : PayPalResponse, new()
            where T1 : AbstractResponseType
        {
            var mrCMSResponse = new T2();
            switch (apiResponse.Ack)
            {
                case AckCodeType.SUCCESS:
                case AckCodeType.SUCCESSWITHWARNING:
                    onSuccess(apiResponse, mrCMSResponse);
                    break;
                default:
                    onFailure(apiResponse, mrCMSResponse);
                    break;
            }
            return mrCMSResponse;
        }

        public static void RaiseErrors<T>(this T type)
            where T : AbstractResponseType
        {
            CurrentRequestData.ErrorSignal.Raise(
                new PayPalException(string.Join(", ",
                                                type.Errors.Select(
                                                    errorType =>
                                                    errorType.ErrorCode + " " +
                                                    errorType.LongMessage))));
        }

        public static Address GetAddress(this AddressType address)
        {
            string[] fullname = address.Name.Trim()
                                                      .Split(new char[] { ' ' }, 2,
                                                             StringSplitOptions.RemoveEmptyEntries);
            string firstName = fullname[0];
            string lastName = string.Empty;
            if (fullname.Length > 1)
                lastName = fullname[1];
            return new Address
                       {
                           Address1 = address.Street1,
                           Address2 = address.Street2,
                           City = address.CityName,
                           Country = GetCountry(address.Country),
                           FirstName = firstName,
                           LastName = lastName,
                           PhoneNumber = address.Phone,
                           PostalCode = address.PostalCode,
                           StateProvince = address.StateOrProvince,
                       };
        }

        private static Country GetCountry(CountryCodeType? country)
        {
            return !country.HasValue
                       ? null
                       : MrCMSApplication.Get<ICountryService>().GetCountryByCode(country.Value.ToString());
        }

        public static BasicAmountType GetAmountType(this decimal value)
        {
            return new BasicAmountType
            {
                value = value.ToString("0.00"),
                currencyID = MrCMSApplication.Get<PayPalExpressCheckoutSettings>().Currency
            };
        }

        public static BasicAmountType GetAmountType(this decimal? value)
        {
            return value.HasValue ? GetAmountType(value.Value) : null;
        }

        public static PaymentStatus GetPaymentStatus(this PaymentStatusCodeType? status)
        {
            if (!status.HasValue)
                return PaymentStatus.Pending;

            switch (status.Value)
            {
                case PaymentStatusCodeType.COMPLETED:
                case PaymentStatusCodeType.PROCESSED:
                    return PaymentStatus.Paid;
                case PaymentStatusCodeType.REFUNDED:
                    return PaymentStatus.Refunded;
                case PaymentStatusCodeType.PARTIALLYREFUNDED:
                    return PaymentStatus.PartiallyRefunded;
                case PaymentStatusCodeType.VOIDED:
                    return PaymentStatus.Voided;
                default:
                    return PaymentStatus.Pending;
            }
        }
    }
}