namespace OS_PayPlug.Components
{
    /// <summary>
    /// Class holding configuration for the Payplug API. 
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Initializes static members of the <see cref="Configuration" /> class.
        /// </summary>
        static Configuration()
        {
            ApiBaseUrl = "https://api.payplug.com";
        }

        /// <summary>
        /// Gets or sets the base URL of the API. 
        /// </summary>
        public static string ApiBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the global authentication secret key. 
        /// </summary>
        public static string SecretKey { get; set; }

        /// <summary>
        /// Gets the authentication header used when making request on the API. 
        /// </summary>
        internal static string AuthorizationHeader
        {
            get
            {
                if (string.IsNullOrEmpty(SecretKey))
                {
                    throw new ConfigurationException("Trying to process a request despite the fact that the secret key was not set.");
                }

                return "Bearer " + SecretKey;
            }
        }

        /// <summary>
        /// Gets or sets the version of the API to use. 
        /// </summary>
        public static string Version { get; set; } = "2019-06-14";
    }
}
