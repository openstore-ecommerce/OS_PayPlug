namespace OS_PayPlug.Components
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A DAO which provides a way to query refund resources.
    /// </summary>
    public static class Refund
    {
        /// <summary>
        /// Create a refund for a given payment.
        /// </summary>
        /// <remarks>To refund a payment, you can either refund the total amount without setting up any parameters,
        /// or partially refund it by providing the amount wanted. 
        /// When a payment is entirely refunded, it will impossible to refund it again.
        /// If you try to refund a payment already entirely refunded you will get an error message.
        /// </remarks>
        /// <seealso cref="CreateRaw">If you want to create a refund using a string of JSON data.</seealso>
        /// <param name="paymentID">ID of the payment to refund.</param>
        /// <param name="refund">Data used in partial refund.</param>
        /// <returns>The created refund.</returns>
        /// <example>
        /// Create a refund from a payment id:
        /// <code><![CDATA[ 
        /// var paymentId = "pay_5iHMDxy4ABR4YBVW4UscIn";
        /// var refundData = new Dictionary<string, dynamic>
        /// {
        ///    {"amount", 3300},
        ///    {"metadata", new Dictionary<string, dynamic>
        ///         {    
        ///             {"customer_id", 42710},
        ///             {"reason", "The delivery was delayed"}
        ///         }
        ///     }
        /// };
        /// var refund = Refund.Create(paymentId, refundData);
        /// ]]></code>
        /// Or even simpler for a total refund:
        /// <code><![CDATA[
        /// var refund = Refund.Create(paymentId);
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Create(string paymentID, Dictionary<string, dynamic> refund = null)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            string json = null;
            if (refund != null)
            {
                json = JsonConvert.SerializeObject(refund);
            }

            var response = CreateRaw(paymentID, json);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Retrieve a refund for a given payment.
        /// </summary>
        /// <seealso cref="RetrieveRaw">If you want to retrieve a refund as a string of JSON data.</seealso>
        /// <param name="paymentID">ID of the refunded payment.</param>
        /// <param name="refundID">ID of the refund to retrieve.</param>
        /// <returns>The refund object asked.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var paymentId = "pay_5iHMDxy4ABR4YBVW4UscIn";
        /// var refundId = "re_3NxGqPfSGMHQgLSZH0Mv3B";
        /// var refund = Refund.Retrieve(paymentId, refundId);
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Retrieve(string paymentID, string refundID)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            if (string.IsNullOrEmpty(refundID))
            {
                throw new ArgumentNullException("refundID cannot be null");
            }

            var response = RetrieveRaw(paymentID, refundID);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Retrieve all refunds for a given payment.
        /// </summary>
        /// <seealso cref="ListRaw">If you want to retrieve refunds as a string of JSON data.</seealso>
        /// <param name="paymentID">ID of the refunds' payment.</param>
        /// <returns>A collection of refunds for a given payment.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var paymentId = "pay_5iHMDxy4ABR4YBVW4UscIn";
        /// var refunds = Refund.List(paymentId);
        /// var refund = refunds["data"][0];
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> List(string paymentID)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            var response = ListRaw(paymentID);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Create a Refund for a given payment with a string of JSON data.
        /// </summary>
        /// <remarks>To refund a payment, you can either refund the total amount without setting up any parameters,
        /// or partially refund it by providing the amount wanted. 
        /// When a payment is entirely refunded, it will impossible to refund it again.
        /// If you try to refund a payment already entirely refunded you will get an error message.
        /// </remarks>
        /// <seealso cref="Create">If you want to create a refund using a Dictionary.</seealso>
        /// <param name="paymentID">ID of the payment to refund.</param>
        /// <param name="refund">Data used in partial refunds as a JSON formatted string.</param>
        /// <returns>The created refund</returns>
        public static string CreateRaw(string paymentID, string refund = null)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "payment_id", paymentID }
            };
            var uri = Routes.Uri(Routes.CreateRefund, parameters);
            return Service.Post(uri, refund);
        }

        /// <summary>
        /// Retrieve a refund for a given payment as a string of JSON data.
        /// </summary>
        /// <seealso cref="Retrieve">If you want to retrieve a refund as a Dictionary.</seealso>
        /// <param name="paymentID">ID of the refunded payment.</param>
        /// <param name="refundID">ID of the refund to retrieve.</param>
        /// <returns>The refunds asked.</returns>
        public static string RetrieveRaw(string paymentID, string refundID)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            if (string.IsNullOrEmpty(refundID))
            {
                throw new ArgumentNullException("refundID cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "payment_id", paymentID },
                { "refund_id", refundID }
            };
            var uri = Routes.Uri(Routes.RetrieveRefund, parameters);
            return Service.Get(uri);
        }

        /// <summary>
        /// Retrieve all refund objects for a given payment as a string of JSON data.
        /// </summary>
        /// <seealso cref="List">If you want to retrieve refunds as a Dictionary.</seealso>
        /// <param name="paymentID">ID of the refunds' payment.</param>
        /// <returns>A collection of refunds for a given payment.</returns>
        /// <example>
        /// <code>
        /// var paymentId = "pay_5iHMDxy4ABR4YBVW4UscIn";
        /// var refunds = Refund.list(paymentId);
        /// var refund = refunds["data"][0];
        /// </code>
        /// </example>
        public static string ListRaw(string paymentID)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "payment_id", paymentID }
            };
            var uri = Routes.Uri(Routes.ListRefunds, parameters);
            return Service.Get(uri);
        }
    }
}
