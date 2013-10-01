using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ryness.Services
{
    public class KerridgeService : IKerridgeService
    {
        private readonly ISession _session;

        public KerridgeService(ISession session)
        {
            _session = session;
        }

        public KerridgeLogPagedList Search(string query = null, int page = 1, int pageSize = 10)
        {
            IPagedList<KerridgeLog> pagedList = _session.Paged(QueryOver.Of<KerridgeLog>().OrderBy(x=>x.Id).Desc, page, pageSize);

            return new KerridgeLogPagedList(pagedList);
        }

        public IList<KerridgeLog> GetAll()
        {
            return _session.QueryOver<KerridgeLog>().Cacheable().List();
        }

        public void Add(KerridgeLog kerridgeLog)
        {
            _session.Transact(session => session.Save(kerridgeLog));
        }

        public void Update(KerridgeLog kerridgeLog)
        {
            _session.Transact(session => session.Update(kerridgeLog));
        }

        public void Delete(KerridgeLog kerridgeLog)
        {
            _session.Transact(session => session.Delete(kerridgeLog));
        }

        public IList<KerridgeLog> GetAllUnsent()
        {
            return _session.QueryOver<KerridgeLog>().Where(x => x.Sent == false).List();
        }

        public bool CanSendToKerridge(KerridgeLog log)
        {
            if (log == null)
                return false;
            return log.Sent == false;
        }

        public bool SendToKerridge(KerridgeLog kerridgeLog)
        {
            //todo check site is live
            var debug = true;
            var o = kerridgeLog.Order;
            if (CanSendToKerridge(kerridgeLog))
            {
                //*********************************************************w
                //*Builds up an order and send it to kerridge             *
                //*                                                       *
                //*********************************************************
                object soapclient = null;
                try
                {
                    string strBranch = "";

                    if (String.IsNullOrEmpty(strBranch))
                        strBranch = "20";

                    string strAccount = "I7002";

                    if (debug == false)
                        strAccount = "I7002";
                    else if (debug == true) //test account
                        strAccount = "I7003";

                    string strReference = "WEB" + o.Id;

                    string strDaterRquired = System.DateTime.Today.ToShortDateString();
                    string newDate = strDaterRquired.Substring(6, 4) + "-" + strDaterRquired.Substring(3, 2) + "-" + strDaterRquired.Substring(0, 2);

                    //*********************************************************
                    //*Strips da1 & ba1 into 3 lines for Kerridge addresas box      *
                    //*                                                       *
                    //*********************************************************


                    string da1Line1 = o.ShippingAddress.Company + " " + o.ShippingAddress.Address1;
                    string da1Line2 = o.ShippingAddress.Address1;
                    string da1Line3 = o.ShippingAddress.City;

                    //*********************************************************

                    string strName = o.ShippingAddress.Name;
                    string strAddressLine1 = "";
                    string strAddressLine2 = "";
                    string strAddressLine3 = "";
                    string strAddressLine4 = "";
                    string strAddressLine5 = "";
                    string strPostCode = "";
                    string strInstructions = "" + " Tel:" + o.ShippingAddress.PhoneNumber;
                    string strCarriage = o.ShippingTotal.ToString();
                    string strCarriageCode = "";

                    string sResponse = "";
                    string strLines = "<lines>";

                    strDaterRquired = newDate;

                    strAddressLine1 = da1Line1;
                    strAddressLine2 = da1Line2;
                    strAddressLine3 = da1Line3;
                    strAddressLine4 = o.ShippingAddress.StateProvince;
                    strAddressLine5 = o.ShippingAddress.Country != null ? o.ShippingAddress.Country.Name : "";
                    strPostCode = o.ShippingAddress.PostalCode;

                    var strHeader = "<header>" + "<branch>" + strBranch + "</branch>" + "<account>" + strAccount + "</account>" + "<reference>" + strReference + "</reference>" +
                        "<daterequired>" + strDaterRquired + "</daterequired>" + "<name>" + strName + " " + o.ShippingAddress.PhoneNumber + "</name>" + "<address>" + "<line>" + strAddressLine1 + "</line>" + "<line>" + strAddressLine2 + "</line>" + "<line>" + strAddressLine3 + "</line>" + "<line>" + strAddressLine4 + "</line>" + "<line>" + strAddressLine5 + "</line>" + "<postcode>" + strPostCode + "</postcode>" + "</address>" + "<instructions>" + strInstructions + "</instructions>" + "</header>";

                    string ModelNumber = "";
                    string QTY = "";
                    decimal UNIT_PRICE = 0;
                    //--------- build product string

                    IList<OrderLine> orderItems = o.OrderLines;

                    foreach (OrderLine item in orderItems)
                    {
                        strLines = strLines + "<line>";
                        string sku = CheckProductCodeLengeth(item.SKU);
                        if (sku == "")
                            sku = "000240";

                        strLines = strLines + "<product>" + sku + "</product>";
                        strLines = strLines + "<quantity>" + item.Quantity + "</quantity>";
                        strLines = strLines + "<daterequired>" + strDaterRquired + "</daterequired>";
                        strLines = strLines + "</line>";
                    }

                    if (o.ShippingTotal != null)
                        switch (Math.Round((decimal)o.ShippingTotal, 2).ToString())
                        {
                            case "1.20":
                                strCarriageCode = "C00010";
                                break;
                            case "1.40":
                                strCarriageCode = "C00011";
                                break;
                            case "1.60":
                                strCarriageCode = "C00012";
                                break;
                            case "1.80":
                                strCarriageCode = "C00013";
                                break;
                            case "2.00":
                                strCarriageCode = "C00014";
                                break;
                            case "2.20":
                                strCarriageCode = "C00015";
                                break;
                            case "2.40":
                                strCarriageCode = "C00016";
                                break;
                            case "2.60":
                                strCarriageCode = "C00017";
                                break;
                            case "2.80":
                                strCarriageCode = "C00018";
                                break;
                            case "3.00":
                                strCarriageCode = "C00019";
                                break;
                            case "1.2":
                                strCarriageCode = "C00010";
                                break;
                            case "1.4":
                                strCarriageCode = "C00011";
                                break;
                            case "1.6":
                                strCarriageCode = "C00012";
                                break;
                            case "1.8":
                                strCarriageCode = "C00013";
                                break;
                            case "2":
                                strCarriageCode = "C00014";
                                break;
                            case "2.2":
                                strCarriageCode = "C00015";
                                break;
                            case "2.4":
                                strCarriageCode = "C00016";
                                break;
                            case "2.6":
                                strCarriageCode = "C00017";
                                break;
                            case "2.8":
                                strCarriageCode = "C00018";
                                break;
                            case "3":
                                strCarriageCode = "C00019";
                                break;
                            case "0.95":
                                strCarriageCode = "C00002";
                                break;
                            case "1.95":
                                strCarriageCode = "C00002";
                                break;
                            case "3.90":
                                strCarriageCode = "C00008";
                                break;
                            case "3.9":
                                strCarriageCode = "C00008";
                                break;
                            case "4.90":
                                strCarriageCode = "C00004";
                                break;
                            case "4.9":
                                strCarriageCode = "C00004";
                                break;
                            case "5.76":
                                strCarriageCode = "C00005";
                                break;
                            case "0.00":
                                strCarriageCode = "C00006";
                                break;
                            case "0":
                                strCarriageCode = "C00006";
                                break;
                            default:
                                strCarriageCode = "C00006";
                                break;
                        }


                    // If shippingcost <> 0 Then

                    strLines = strLines + "<line>";
                    strLines = strLines + "<product>" + strCarriageCode + "</product>";
                    strLines = strLines + "<quantity>1</quantity>";
                    strLines = strLines + "<daterequired>" + strDaterRquired + "</daterequired>";
                    strLines = strLines + "</line>";

                    strLines += "</lines>";

                    string result = "";
                    string xmlOrderObject = "";
                    if (debug == false)
                    {
                        xmlOrderObject = "<kmsgsimple id='2' password='websales'>" + strHeader + strLines + "</kmsgsimple>";
                    }
                    else if (debug == true)
                    {
                        xmlOrderObject = "<kmsgsimple id='EBAY' password='xxx'>" + strHeader + strLines + "</kmsgsimple>";
                    }

                    return CallKerridge(xmlOrderObject);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        private string CheckProductCodeLengeth(string sku)
        {
            if (!string.IsNullOrEmpty(sku))
            {
                //------ adds 0 to product number until product number is 6 char long
                if (sku.Length < 6)
                {
                    while ((sku.Length) < 6)
                    {
                        sku = "0" + sku;
                    }
                }

                if (sku.LastIndexOf("-", System.StringComparison.Ordinal) > 0)
                {
                    sku = sku.Substring(0, sku.LastIndexOf("-", System.StringComparison.Ordinal));
                }

                if (sku.Length >= 8)
                {
                    sku = "000240";
                }
            }

            return sku;
        }

        bool CallKerridge(string xmlString)
        {
            string url = "http://mail.ryness.co.uk/KERRIDGE_INTEGRATION/KerridgeOrderHandlerV2Ebay.aspx";
            // url = "http://mail.ryness.co.uk/KERRIDGE_INTEGRATION/KerridgeOrderHandlerV2Ebay.aspx";
            // url = "http://mail.ryness.co.uk/KERRIDGE_INTEGRATION/KerridgeOrderHandlerV2.aspx";

            url = url + "?xmlstring=" + System.Web.HttpUtility.UrlEncode(xmlString);
            var xmlmessage = "xmlstring=" + System.Web.HttpUtility.HtmlEncode(xmlString);

            WebRequest objRequest = WebRequest.Create(url);

            objRequest.Headers.Add("Action", "PlaceOrder");
            objRequest.Method = "POST";
            objRequest.ContentLength = xmlmessage.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            var myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(xmlmessage);

            myWriter.Close();

            try
            {
                objRequest.GetResponse();
            }
            catch (WebException ex)
            {
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }

            objRequest.Abort();

            return true;
        }
    }
}