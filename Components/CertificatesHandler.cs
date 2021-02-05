using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using OS_PayPlug;

namespace OS_PayPlug.Components
{
    internal static class CertificatesHandler
    {
        /// <summary>
        /// Collection of certificate.
        /// </summary>
        private static X509Certificate2Collection certificate2Collection;

        /// <summary>
        /// Initializes static members of the <see cref="CertificatesHandler" /> class.
        /// </summary>
        static CertificatesHandler()
        {
            var cacert = System.Text.Encoding.Default.GetString(OS_PayPlug.Resource.cacert);
            var rawPemCertificates = new Regex(
                @"-----BEGIN CERTIFICATE-----((?:.*\s)*?)-----END CERTIFICATE-----",
                RegexOptions.Compiled | RegexOptions.Multiline).Matches(cacert);

            certificate2Collection = new X509Certificate2Collection();
            foreach (Match rawPemCertificate in rawPemCertificates)
            {
                certificate2Collection.Add(new X509Certificate2(Convert.FromBase64String(rawPemCertificate.Groups[1].Value)));
            }
        }

        /// <summary>
        /// Callback used by ServicePointManager to handle certificates validation.
        /// </summary>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            X509Chain privateChain = new X509Chain();
            privateChain.ChainPolicy.RevocationMode = X509RevocationMode.Offline;
            privateChain.ChainPolicy.ExtraStore.AddRange(certificate2Collection);
            privateChain.Build(new X509Certificate2(certificate));

            foreach (X509ChainStatus chainStatus in privateChain.ChainStatus)
            {
                if (chainStatus.Status != X509ChainStatusFlags.NoError)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
