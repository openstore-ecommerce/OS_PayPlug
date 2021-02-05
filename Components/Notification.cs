namespace OS_PayPlug.Components
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Newtonsoft.Json;

    /// <summary>
    /// Class used to process notifications.
    /// </summary>
    public static class Notification
    {
        /// <summary>
        /// Treat a notification and guarantee its authenticity.
        /// </summary>
        /// <seealso cref="TreatRaw">If you want to retrieve a payment as a string of JSON data.</seealso>  
        /// <param name="payment">The request body in plain text.</param>
        /// <returns>Trusted payment resource.</returns>
        /// <example>
        /// <code><![CDATA[
        /// string json = new StreamReader(request.InputStream).ReadToEnd();  // This line may depend on your code.
        /// try
        /// {
        ///     var payment = Notification.Treat(json);
        ///     if (payment["object"] == "payment" && payment["is_paid"])
        ///     {
        ///         // Process the paid payment
        ///     }
        ///     else if (payment["object"] == "refund")
        ///     {
        ///         // Process the refund
        ///     }
        /// } 
        /// catch (InvalidApiResourceException e)
        /// {
        ///     // Handle errors
        /// }
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Treat(string payment)
        {
            if (string.IsNullOrEmpty(payment))
            {
                throw new ArgumentNullException("payment cannot be null.");
            }

            string paymentID;
            try
            {
                paymentID = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(payment)["id"];
                return PaymentUtils.Retrieve(paymentID);
            }
            catch (JsonSerializationException)
            {
                throw new InvalidApiResourceException("Request body is malformed JSON.");
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidApiResourceException("The API resource provided is invalid.");
            }
            catch (ClientWebException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new InvalidApiResourceException("The resource you requested could not be found.");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Treat a notification and guarantee its authenticity.
        /// </summary>
        /// <seealso cref="Treat">If you want to retrieve a payment as a dictionary.</seealso>  
        /// <param name="payment">The request body in plain text.</param>
        /// <returns>Trusted payment resource.</returns>
        public static string TreatRaw(string payment)
        {
            if (string.IsNullOrEmpty(payment))
            {
                throw new ArgumentNullException("payment cannot be null.");
            }

            string paymentID;
            try
            {
                paymentID = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(payment)["id"];
                return PaymentUtils.RetrieveRaw(paymentID);
            }
            catch (JsonSerializationException)
            {
                throw new InvalidApiResourceException("Request body is malformed JSON.");
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidApiResourceException("The API resource provided is invalid.");
            }
            catch (ClientWebException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new InvalidApiResourceException("The resource you requested could not be found.");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
