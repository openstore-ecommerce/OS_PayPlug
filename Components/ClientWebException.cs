using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OS_PayPlug.Components
{
    /// <summary>
    /// Exception thrown when a request received a client error (4xx).
    /// </summary>
    [System.Serializable]
    public class ClientWebException : WebException
    {
        public ClientWebException()
        {
        }

        public ClientWebException(string message) : base(message)
        {
        }

        public ClientWebException(string message, System.Exception inner) : base(message, inner)
        {
        }

        public ClientWebException(WebException inner) : base(GetMessageFromWebResponse(inner), inner, inner.Status, inner.Response)
        {
            var webResponse = inner.Response as HttpWebResponse;
            this.StatusCode = webResponse.StatusCode;
        }

        protected ClientWebException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets the http status code received.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }

        private static string GetMessageFromWebResponse(WebException e)
        {
            string message;
            var webResponse = e.Response as HttpWebResponse;

            using (var sr = new StreamReader(webResponse.GetResponseStream()))
            {
                try
                {
                    var response = sr.ReadToEnd();
                    message = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response)["message"];
                }
                catch (Exception)
                {
                    message = e.Message;
                }
            }

            webResponse.Close();
            return message;
        }
    }
}
