using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using NBrightCore.common;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;
using DotNetNuke.Common.Utilities;

namespace OS_PayPlug
{
    public class ProviderUtils
    {
        public static NBrightInfo GetProviderSettings()
        {
            var objCtrl = new NBrightBuyController();
            var info = objCtrl.GetPluginSinglePageData("OSPayPalpayment", "OSPayPalPAYMENT", Utils.GetCurrentCulture());
            return info;
        }


        public static String GetBankRemotePost(OrderData orderData)
        {

            var rPost = new RemotePost();

            var objCtrl = new NBrightBuyController();
            var settings = objCtrl.GetPluginSinglePageData("OSPayPalpayment", "OSPayPalPAYMENT", orderData.Lang);


            // get the order data
            var payData = new PayData(orderData);

            rPost.Url = payData.PostUrl;

            rPost.Add("cmd", "_xclick");
            rPost.Add("item_number", payData.ItemId);
            rPost.Add("return", payData.ReturnUrl);
            rPost.Add("currency_code", payData.CurrencyCode);
            rPost.Add("cancel_return", payData.ReturnCancelUrl);
            rPost.Add("notify_url", payData.NotifyUrl);
            rPost.Add("custom", Utils.GetCurrentCulture());
            rPost.Add("business", payData.PayPalId);
            rPost.Add("item_name", orderData.PurchaseInfo.GetXmlProperty("genxml/ordernumber"));
            rPost.Add("amount", payData.Amount);
            rPost.Add("shipping", payData.ShippingAmount);
            rPost.Add("tax", payData.TaxAmount);
            rPost.Add("lc", Utils.GetCurrentCulture().Substring(3, 2));

            var extrafields = settings.GetXmlProperty("genxml/textbox/extrafields");
            var fields = extrafields.Split(',');
            foreach (var f in fields)
            {
                var ary = f.Split('=');
                if (ary.Count() == 2)
                {
                    var n = ary[0];
                    var v = ary[1];
                    var d = orderData.PurchaseInfo.GetXmlProperty(v);
                    rPost.Add(n, d);
                }
            }

            //Build the re-direct html 
            var rtnStr = "";
            rtnStr = rPost.GetPostHtml();

            if (settings.GetXmlPropertyBool("genxml/checkbox/debugmode"))
            {
                File.WriteAllText(PortalSettings.Current.HomeDirectoryMapPath + "\\debug_OSPayPalpost.html", rtnStr);
            }
            return rtnStr;
        }



        public static bool VerifyPayment(PayPalIpnParameters ipn, string verifyURL)
        {
            try
            {
                bool isVerified = false;

                if (ipn.IsValid)
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                    HttpWebRequest PPrequest = (HttpWebRequest)WebRequest.Create(verifyURL);
                    if ((PPrequest != null))
                    {
                        PPrequest.Method = "POST";
                        PPrequest.ContentLength = ipn.PostString.Length;
                        PPrequest.ContentType = "application/x-www-form-urlencoded";
                        StreamWriter writer = new StreamWriter(PPrequest.GetRequestStream());
                        writer.Write(ipn.PostString);
                        writer.Close();
                        HttpWebResponse response = (HttpWebResponse)PPrequest.GetResponse();
                        if ((response != null))
                        {
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            string responseString = reader.ReadToEnd();
                            reader.Close();
                            if (string.Compare(responseString, "VERIFIED", true) == 0)
                            {
                                isVerified = true;
                            }
                        }
                    }
                }
                return isVerified;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string PayPalEncode(string value)
        {
            //a single accentuated/special character matches a single non acc/spec character:
            value = StringListReplace(value, "ŠŽŸÀÁÂÃÅÇÈÉÊËÌÍÎÏÐÑÒÓÔÕØÙÚÛÝÞØ", "SZYAAAAACEEEEIIIIDNOOOOOUUUYPO");
            value = StringListReplace(value, "šžÿàáâãåçèéêëìíîïðñòóôõøùúûýþµ", "szyaaaaaceeeeiiiidnooooouuuypu");

            //a single accentuated/special character matches a couple of non acc/spec character:
            value = value.Replace("Œ", "OE");
            value = value.Replace("Æ", "AE");
            value = value.Replace("œ", "oe");
            value = value.Replace("æ", "ae");

            return HttpUtility.UrlEncode(value);
        }

        private string StringListReplace(string value, string searchfor, string replacewith)
        {
            for (var x = 1; x <= searchfor.Length; x++)
            {
                value = value.Replace(searchfor.Substring(x - 1, 1), replacewith.Substring(x - 1, 1));
            }
            return value;
        }

    }
}
