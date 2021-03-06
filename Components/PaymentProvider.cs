﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using NBrightCore.common;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;

namespace OS_PayPlug
{
    public class OSPayPlugPaymentProvider : Nevoweb.DNN.NBrightBuy.Components.Interfaces.PaymentsInterface
    {
        public override string Paymentskey { get; set; }

        public override string GetTemplate(NBrightInfo cartInfo)
        {
            var templ = "";
            var objCtrl = new NBrightBuyController();
            var info = objCtrl.GetPluginSinglePageData("OSPayPlugpayment", "OSPayPlugPAYMENT", Utils.GetCurrentCulture());
            var templateName = info.GetXmlProperty("genxml/textbox/checkouttemplate");
            var passSettings = info.ToDictionary();
            foreach (var s in StoreSettings.Current.Settings()) // copy store setting, otherwise we get a byRef assignement
            {
                if (passSettings.ContainsKey(s.Key))
                    passSettings[s.Key] = s.Value;
                else
                    passSettings.Add(s.Key, s.Value);
            }
            templ = NBrightBuyUtils.RazorTemplRender(templateName, 0, "", info, "/DesktopModules/NBright/OS_PayPlug", "config", Utils.GetCurrentCulture(), passSettings);

            return templ;
        }

        public override string RedirectForPayment(OrderData orderData)
        {

            orderData.OrderStatus = "020";
            orderData.PurchaseInfo.SetXmlProperty("genxml/paymenterror", "");
            orderData.PurchaseInfo.Lang = Utils.GetCurrentCulture();
            orderData.SavePurchaseData();
            try
            {

                 var payPlugData = new PayPlugLimpet(orderData);

                var paymentkey = payPlugData.CreatePayment();
                if (paymentkey != null && paymentkey.ContainsKey("hosted_payment"))
                {
                    var hosted_payment = paymentkey["hosted_payment"];
                    if (hosted_payment != null)
                    {
                        orderData.PurchaseInfo.SetXmlProperty("genxml/posturl", hosted_payment["payment_url"].Value);
                        orderData.PurchaseInfo.SetXmlProperty("genxml/paymentkey", paymentkey["id"]);
                        orderData.Save();

                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.Write(ProviderUtils.GetBankRemotePost(orderData));
                    }
                }
            }
            catch (Exception ex)
            {
                // rollback transaction
                orderData.PurchaseInfo.SetXmlProperty("genxml/paymenterror", "<div>ERROR: Invalid payment data </div><div>" + ex + "</div>");
                orderData.PaymentFail();
                var param = new string[3];
                param[0] = "orderid=" + orderData.PurchaseInfo.ItemID.ToString("");
                param[1] = "status=0";
                return Globals.NavigateURL(StoreSettings.Current.PaymentTabId, "", param);
            }

            try
            {
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                // this try/catch to avoid sending error 'ThreadAbortException'  
            }

            return "";
        }

        public override string ProcessPaymentReturn(HttpContext context)
        {
            var orderid = Utils.RequestQueryStringParam(context, "orderid");
            if (Utils.IsNumeric(orderid))
            {
                var status = Utils.RequestQueryStringParam(context, "status");
                if (status == "0")
                {
                    var orderData = new OrderData(Convert.ToInt32(orderid));
                    if (orderData.OrderStatus == "020") // check we have a waiting for bank status (Cancel from bank seems to happen even after notified has accepted it as valid??)
                    {
                        var rtnerr = orderData.PurchaseInfo.GetXmlProperty("genxml/paymenterror");
                        orderData.PaymentFail();
                        return ""; // empty trigger fail message.
                    }
                }
            }
            return "";
        }


        private string GetReturnTemplate(OrderData orderData, bool paymentok, string paymenterror)
        {
            var info = ProviderUtils.GetProviderSettings();
            info.UserId = UserController.Instance.GetCurrentUserInfo().UserID;
            var templ = "";
            var passSettings = NBrightBuyUtils.GetPassSettings(info);
            if (passSettings.ContainsKey("paymenterror"))
            {
                passSettings.Add("paymenterror", paymenterror);
            }
            var displaytemplate = "payment_ok.cshtml";
            if (paymentok)
            {
                info.SetXmlProperty("genxml/ordernumber", orderData.OrderNumber);
                templ = NBrightBuyUtils.RazorTemplRender(displaytemplate, 0, "", info, "/DesktopModules/NBright/OS_PayPlug", "config", Utils.GetCurrentCulture(), passSettings);
            }
            else
            {
                displaytemplate = "payment_fail.cshtml";
                templ = NBrightBuyUtils.RazorTemplRender(displaytemplate, 0, "", info, "/DesktopModules/NBright/OS_PayPlug", "config", Utils.GetCurrentCulture(), passSettings);
            }

            return templ;
        }

    }
}
