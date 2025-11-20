using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace GlobalPayments.Api.Entities {
    internal class GpApiSessionInfo : IAccessTokenProvider {

        /// <summary>
        /// A unique string created using the nonce and app-key.
        /// This value is used to further authenticate the request.
        /// Created as follows - SHA512(NONCE + APP-KEY).
        /// </summary>
        /// <param name="input">string to hash</param>
        /// <returns>hash of the input string</returns>
        private static string GenerateSecret(string nonce, string appKey) {
            byte[] data = Encoding.UTF8.GetBytes(nonce + appKey);

            using (SHA512 shaM = SHA512.Create()) {
                byte[] hash = shaM.ComputeHash(data);

                StringBuilder sb = new StringBuilder(64);
                foreach (var b in hash) {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString().ToLower();
            }
        }

        public Request SignIn(string appId, string appKey, int? secondsToExpire = null, IntervalToExpire? intervalToExpire = null, string[] permissions = null, PorticoTokenConfig porticoTokenConfig = null) {
            string nonce = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt");

            var request = new JsonDoc();
            var credentials = new List<Dictionary<string, string>>();

            bool hasPorticoConfig = porticoTokenConfig != null;
            bool hasMainCredentials =
                hasPorticoConfig &&
                porticoTokenConfig.SiteId != default(int) &&
                porticoTokenConfig.LicenseId != default(int) &&
                porticoTokenConfig.DeviceId != default(int) &&
                !string.IsNullOrEmpty(porticoTokenConfig.Username) &&
                !string.IsNullOrEmpty(porticoTokenConfig.Password);

            bool hasApiKey = hasPorticoConfig && !string.IsNullOrEmpty(porticoTokenConfig.SecretApiKey);

            if (hasMainCredentials) {
                credentials.AddRange(new[] {
                        new Dictionary<string, string> { { "name", "device_id" }, { "value", porticoTokenConfig.DeviceId.ToString() } },
                        new Dictionary<string, string> { { "name", "site_id" }, { "value", porticoTokenConfig.SiteId.ToString() } },
                        new Dictionary<string, string> { { "name", "license_id" }, { "value", porticoTokenConfig.LicenseId.ToString() } },
                        new Dictionary<string, string> { { "name", "username" }, { "value", porticoTokenConfig.Username } },
                        new Dictionary<string, string> { { "name", "password" }, { "value", porticoTokenConfig.Password } }
                });

                if (hasApiKey) {
                    credentials.Add(new Dictionary<string, string> { { "name", "apikey" }, { "value", porticoTokenConfig.SecretApiKey } });
                }

                if (!string.IsNullOrEmpty(appId)) {
                    request.Set("app_id", appId);
                }
                request.Set("credentials", credentials);
                request.Set("grant_type", "client_credentials");
            }
            else if (hasApiKey) {

                credentials.Add(new Dictionary<string, string> { { "name", "apikey" }, { "value", porticoTokenConfig.SecretApiKey } });

                if (!string.IsNullOrEmpty(appId)) {
                    request.Set("app_id", appId);
                }
                request.Set("credentials", credentials);
                request.Set("grant_type", "client_credentials");
            }
            else {
                request = new JsonDoc()
                    .Set("app_id", appId)
                    .Set("nonce", nonce)
                    .Set("grant_type", "client_credentials")
                    .Set("secret", GenerateSecret(nonce, appKey))
                    .Set("seconds_to_expire", secondsToExpire)
                    .Set("interval_to_expire", EnumConverter.GetMapping(Target.GP_API, intervalToExpire))
                    .Set("permissions", permissions);
            }
            return new Request {
                Verb = HttpMethod.Post,
                Endpoint = $"{GpApiRequest.ACCESS_TOKEN_ENDPOINT}",
                RequestBody = request.ToString()
            };
        }

        public Request SignOut() {
            throw new Exception("SignOut not implemented");

            //return new PayrollRequest
            //{
            //    Endpoint = "/api/pos/session/signout"
            //};
        }
    }
}
