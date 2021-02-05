using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_PayPlug.Components
{
    public static class PaymentUtils
    {
        /// <summary>
        /// Create a payment request.
        /// </summary>
        /// <seealso cref="CreateRaw">If you want to create a payment using a string of JSON data.</seealso>
        /// <param name="payment">Data used to create the payment.</param>
        /// <returns>The created payment.</returns>
        /// <example>
        /// <code><![CDATA[ 
        /// var paymentData = new Dictionary<string, dynamic>
        /// {
        ///     {"amount", 3300},
        ///     {"currency", "EUR"},
        ///     {"customer", new Dictionary<string, object>
        ///         {
        ///             {"email", "john.watson@example.net"},
        ///             {"first_name", "John"},
        ///             {"last_name", "Watson"}
        ///         }
        ///     },
        ///     {"hosted_payment", new Dictionary<string, object>
        ///         {
        ///             {"return_url", "https://example.net/success?id=42710"},
        ///             {"cancel_url", "https://example.net/cancel?id=42710"}
        ///         }
        ///     },
        ///     {"notification_url", "https://example.net/notifications?id=42710"},
        ///     {"metadata", new Dictionary<string, object>
        ///         {
        ///             {"customer_id", "42710"}
        ///         }
        ///     },
        ///     {"save_card", false},
        ///     {"force_3ds", true}
        /// };
        /// var payment = Payment.Create(paymentData);
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Create(Dictionary<string, object> payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException("payment cannot be null");
            }

            var json = JsonConvert.SerializeObject(payment);
            var response = CreateRaw(json);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Retrieve a payment.
        /// </summary>
        /// <seealso cref="RetrieveRaw">If you want to retrieve a payment as a string of JSON data.</seealso>
        /// <param name="paymentID">ID of the payment to retrieve.</param>
        /// <returns>The payment asked.</returns>
        /// <example>
        /// <code><![CDATA[ 
        /// var payment = Payment.Retrieve("pay_5iHMDxy4ABR4YBVW4UscIn");
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Retrieve(string paymentID)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            var response = RetrieveRaw(paymentID);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Retrieve all payments.
        /// </summary>
        /// <seealso cref="ListRaw">If you want to retrieve payments  as a string of JSON data.</seealso>
        /// <param name="page">The page number.</param>
        /// <param name="per_page">number of payment per page.</param>
        /// <returns>A collection of payments.</returns>       
        /// <example>
        /// <code><![CDATA[ 
        /// var payments = Payment.List();
        /// var payment = payments["data"][0];
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> List(uint? page = null, uint? per_page = null)
        {
            var response = ListRaw(page, per_page);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Abort a payment request.
        /// </summary>
        /// <param name="paymentID">ID of the payment to abort.</param>
        /// <returns>The aborted payment.</returns>        
        /// <example>
        /// <code><![CDATA[
        /// var payment = Payment.Abort("pay_5iHMDxy4ABR4YBVW4UscIn");
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Abort(string paymentID)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "payment_id", paymentID }
            };
            var uri = Routes.Uri(Routes.AbortPayments, parameters);
            var data = new Dictionary<string, dynamic>
            {
                { "abort", true }
            };
            var json = JsonConvert.SerializeObject(data);
            var response = Service.Patch(uri, json);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Create a payment request with a string of JSON data.
        /// </summary>
        /// <seealso cref="Create">If you want to create a payment request using a Dictionary.</seealso>
        /// <param name="payment">Data used to create the payment as a string of JSON data. </param>
        /// <returns>The created payment.</returns>
        public static string CreateRaw(string payment)
        {
            if (string.IsNullOrEmpty(payment))
            {
                throw new ArgumentNullException("payment cannot be null");
            }

            var uri = Routes.Uri(Routes.CreatePayment);
            return Service.Post(uri, payment);
        }

        /// <summary>
        /// Retrieve a payment as a string of JSON data.
        /// </summary>
        /// <seealso cref="Retrieve">If you want to retrieve a payment as a Dictionary.</seealso>
        /// <param name="paymentID">ID of the payment to retrieve.</param>
        /// <returns>The payment asked.</returns>
        public static string RetrieveRaw(string paymentID)
        {
            if (string.IsNullOrEmpty(paymentID))
            {
                throw new ArgumentNullException("paymentID cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "payment_id", paymentID }
            };
            var uri = Routes.Uri(Routes.RetrievePayment, parameters);
            return Service.Get(uri);
        }

        /// <summary>
        /// Retrieve all payments as a string of JSON data.
        /// </summary>
        /// <seealso cref="List">If you want to retrieve payments as a Dictionary.</seealso>
        /// <param name="page">The page number.</param>
        /// <param name="per_page">number of payment per page.</param>
        /// <returns>A collection of payments.</returns>
        public static string ListRaw(uint? page = null, uint? per_page = null)
        {
            var query = new Dictionary<string, string>();
            if (page != null)
            {
                query.Add("page", page.ToString());
            }

            if (per_page != null)
            {
                query.Add("per_page", per_page.ToString());
            }

            var uri = Routes.Uri(Routes.ListPayments, null, query);
            return Service.Get(uri);
        }
    }

}
