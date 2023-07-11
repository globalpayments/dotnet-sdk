using GlobalPayments.Api.Utils;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using GlobalPayments.Api.Entities.GpApi;

namespace GlobalPayments.Api.Entities {
    internal class GpApiSessionInfo {

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

        internal static Request SignIn(string appId, string appKey, int? secondsToExpire = null, IntervalToExpire? intervalToExpire = null, string[] permissions = null) {
            string nonce = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            
            var request = new JsonDoc()
                .Set("app_id", appId)
                .Set("nonce", nonce)
                .Set("grant_type", "client_credentials")
                .Set("secret", GenerateSecret(nonce, appKey))
                .Set("seconds_to_expire", secondsToExpire)
                .Set("interval_to_expire", EnumConverter.GetMapping(Target.GP_API, intervalToExpire))
                .Set("permissions", permissions);

            return new Request {
                Verb = HttpMethod.Post,
                Endpoint = $"{GpApiRequest.ACCESS_TOKEN_ENDPOINT}",
                RequestBody = request.ToString()
            };
        }

        internal static Request SignOut() {
            throw new Exception("SignOut not implemented");

            //return new PayrollRequest
            //{
            //    Endpoint = "/api/pos/session/signout"
            //};
        }
    }
}
