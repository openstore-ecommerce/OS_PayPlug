using System;
using System.IO;
using System.Net;
using System.Text;

namespace OS_PayPlug.Components
{
    internal static class Service
    {
        /// <summary>
        /// Send an authenticated GET request.
        /// </summary>
        /// <param name="uri">The URI to the resource queried.</param>
        /// <returns>The response content.</returns>
        public static string Get(Uri uri)
        {
            return Request("GET", uri, null, Configuration.AuthorizationHeader, Configuration.Version);
        }

        /// <summary>
        /// Send an authenticated POST request.
        /// </summary>
        /// <param name="uri">The URI to the resource queried.</param>
        /// <param name="data">The request content.</param>
        /// <returns>The response content.</returns>
        public static string Post(Uri uri, string data)
        {
            return Request("POST", uri, data, Configuration.AuthorizationHeader, Configuration.Version);
        }

        /// <summary>
        /// Send an authenticated PATCH request.
        /// </summary>
        /// <param name="uri">The URI to the resource queried.</param>
        /// <param name="data">The request content.</param>
        /// <returns>The response content.</returns>
        public static string Patch(Uri uri, string data)
        {
            return Request("PATCH", uri, data, Configuration.AuthorizationHeader, Configuration.Version);
        }

        /// <summary>
        /// Send an authenticated DELETE request.
        /// </summary>
        /// <param name="uri">The URI to the resource queried.</param>
        /// <param name="data">The request content.</param>
        public static void Delete(Uri uri, string data = null)
        {
            Request("DELETE", uri, data, Configuration.AuthorizationHeader, Configuration.Version);
        }

        /// <summary>
        /// Perform an HTTP request.
        /// </summary>
        /// <param name="method">The HTTP verb (GET, POST, PUT, …).</param>
        /// <param name="uri">The URI to the resource queried.</param>
        /// <param name="data">The request content.</param>
        /// <param name="authorizationHeader">Authorization header used to authenticate the request.</param>
        /// <returns>The response content.</returns>
        private static string Request(string method, Uri uri, string data, string authorizationHeader = null, string version = null)
        {
#if !__MonoCS__
            if ((System.Net.ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls) != SecurityProtocolType.Tls &&
                (System.Net.ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls11) != SecurityProtocolType.Tls11 &&
                (System.Net.ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) != SecurityProtocolType.Tls12)
#else
            if ((System.Net.ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls) != SecurityProtocolType.Tls)
#endif
            {
                throw new ConfigurationException("Trying to process a request despite the fact that TLSv1+ was not enabled.");
            }

            var oldCallback = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = CertificatesHandler.ValidateServerCertificate;

            try
            {
                var request = WebRequest.Create(uri) as HttpWebRequest;
                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    request.Headers.Add("Authorization", authorizationHeader);
                }
                if (!string.IsNullOrEmpty(version))
                {
                    request.Headers.Add("PayPlug-Version", version);
                }

                request.Accept = "application/json";
                request.UserAgent = string.Format("PayPlug-Sharp/{0}", typeof(Service).Assembly.GetName().Version.ToString());
                request.Method = method;
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    request.ContentType = "application/json";
                    request.ContentLength = buffer.Length;
                    using (var rs = request.GetRequestStream())
                    {
                        rs.Write(buffer, 0, buffer.Length);
                    }
                }

                string response;
                var webResponse = request.GetResponse() as HttpWebResponse;
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    response = sr.ReadToEnd();
                }

                webResponse.Close();
                return response;
            }
            catch (WebException e)
            {
                // Throw the exception again if it's not a protocol error
                if (e.Status != WebExceptionStatus.ProtocolError)
                {
                    throw;
                }

                // Map a response to an Exception to throw
                throw MapHTTPStatusToException(e);
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = oldCallback;
            }
        }

        /// <summary>
        /// Map HTTP client error status to exceptions.
        /// </summary>
        /// <param name="e">Exception that was raised by the WebRequest.</param>
        /// <returns>Exception to be raised.</returns>
        private static WebException MapHTTPStatusToException(WebException e)
        {
            WebException exception;

            // Get the response from the server
            var webResponse = e.Response as HttpWebResponse;

            // Map to ClientWebException only on 4xx status code
            if ((int)webResponse.StatusCode >= 400 && (int)webResponse.StatusCode < 500)
            {
                // Used if we want to introduce new Exceptions subclass
                switch (webResponse.StatusCode)
                {
                    // For now everything is a ClientWebException
                    default:
                        exception = new ClientWebException(e);
                        break;
                }
            }
            else
            {
                exception = e;
            }

            webResponse.Close();
            return exception;
        }
    }
}
