using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OS_PayPlug.Components
{
    internal static class Routes
    {
        // Payments routes

        /// <summary>
        /// Route path used to create payments.
        /// </summary>
        public const string CreatePayment = "/payments";

        /// <summary>
        /// Route path used to retrieve payments.
        /// </summary>
        public const string RetrievePayment = "/payments/{payment_id}";

        /// <summary>
        /// Route path used to list payments. 
        /// </summary>
        public const string ListPayments = "/payments";

        /// <summary>
        /// Route path used to abort payments. 
        /// </summary>
        public const string AbortPayments = "/payments/{payment_id}";

        // Refunds routes

        /// <summary>
        /// Route path used to create refunds. 
        /// </summary>
        public const string CreateRefund = "/payments/{payment_id}/refunds";

        /// <summary>
        /// Route path used to retrieve refunds. 
        /// </summary>
        public const string RetrieveRefund = "/payments/{payment_id}/refunds/{refund_id}";

        /// <summary>
        /// Route path used to list refunds.
        /// </summary>
        public const string ListRefunds = "/payments/{payment_id}/refunds";

        // Customer routes

        /// <summary>
        /// Route path used to create customers.
        /// </summary>
        public const string CreateCustomer = "/customers/{customer_id}";

        /// <summary>
        /// Route path used to retrieve customers.
        /// </summary>
        public const string RetrieveCustomer = "/customers/{customer_id}";

        /// <summary>
        /// Route path used to list customers.
        /// </summary>
        public const string ListCustomers = "/customers";

        /// <summary>
        /// Route path used to update customers.
        /// </summary>
        public const string UpdateCustomer = "/customers/{customer_id}";

        /// <summary>
        /// Route path used to delete customers.
        /// </summary>
        public const string DeleteCustomer = "/customers/{customer_id}";

        // Card routes

        /// <summary>
        /// Route path used to save cards to a customer.
        /// </summary>
        public const string CreateCard = "/customers/{customer_id}/cards";

        /// <summary>
        /// Route path used to retrieve cards.
        /// </summary>
        public const string RetrieveCard = "/customers/{customer_id}/cards/{card_token}";

        /// <summary>
        /// Route path used to list cards for customers.
        /// </summary>
        public const string ListCards = "/customers/{customer_id}/cards";

        /// <summary>
        /// Route path used to delete cards.
        /// </summary>
        public const string DeleteCard = "/customers/{customer_id}/cards/{card_token}";

        /// <summary>
        /// Version of the API used in the URLs.
        /// </summary>
        private const string ApiVersion = "1";

        /// <summary>
        /// Generates an absolute URL to an API resource.
        /// </summary>
        /// <param name="path">The path format to access a resource.</param>
        /// <param name="pathParameters">Parameters required by the route.</param>
        /// <param name="queryParameters">Parameters of the query string.</param>
        /// <returns>The URI to access a resource.</returns>
        public static Uri Uri(string path, Dictionary<string, string> pathParameters = null, Dictionary<string, string> queryParameters = null)
        {
            var absolute = new Uri(Configuration.ApiBaseUrl);
            var relative = "/v" + ApiVersion;
            if (path != null)
            {
                if (!path.StartsWith("/"))
                {
                    relative = relative + "/";
                }

                relative = relative + path.FormatFromDictionary(pathParameters);
            }

            if (queryParameters != null)
            {
                relative = relative + ToQueryString(queryParameters);
            }

            return new Uri(absolute, relative);
        }

        /// <summary>
        /// Helper that provide a way to use named string parameter.
        /// </summary>
        /// <param name="formatString">The string to be formatted</param>
        /// <param name="valueDict">A dictionary used to replace the named parameter by some values.</param>
        /// <returns>The formatted string.</returns>
        private static string FormatFromDictionary(this string formatString, Dictionary<string, string> valueDict)
        {
            if (valueDict != null)
            {
                foreach (var tuple in valueDict)
                {
                    formatString = formatString.Replace("{" + tuple.Key + "}", tuple.Value);
                }
            }

            return formatString;
        }

        /// <summary>
        /// Helper that provide a way to build a query string.
        /// </summary>
        /// <param name="queryParameters">Keys values used to build the query string.</param>
        /// <returns>A query string.</returns>
        private static string ToQueryString(Dictionary<string, string> queryParameters)
        {
            if (queryParameters.Count == 0)
            {
                return string.Empty;
            }

            var query = new List<string>();
            foreach (KeyValuePair<string, string> entry in queryParameters)
            {
                query.Add(string.Format("{0}={1}", HttpUtility.UrlEncode(entry.Key), HttpUtility.UrlEncode(entry.Value)));
            }

            return "?" + string.Join("&", query);
        }
    }
}
