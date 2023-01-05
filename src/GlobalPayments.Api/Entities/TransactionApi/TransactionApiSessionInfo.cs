using GlobalPayments.Api.Utils;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using Newtonsoft.Json;

namespace GlobalPayments.Api.Entities  {
    internal class TransactionApiSessionInfo {

        internal string SignIn(string region, string accountCredential, string apiSecret) {
            return GenerateEncodedAuthToken(accountCredential, region, apiSecret);
        }

        public string GenerateEncodedAuthToken(string accountCredential, string region, string apiSecret) {
            if (accountCredential == null || region == null || string.IsNullOrEmpty(apiSecret)) {
                throw new Exception("MANDATORY INPUT DATA IS EMPTY");
            }
            try {
                string encodedHeaderJSON = GenerateEncodedJSONHeader();
                string encodedPayLoadJSON = GenerateEncodedPayloadJSON(accountCredential, region);
                string encodedSignature = GenerateHashSignature(encodedHeaderJSON, encodedPayLoadJSON, apiSecret);
                string GeneratedAuthTokenV2 = encodedHeaderJSON + "." + encodedPayLoadJSON + "." + encodedSignature;
                return GeneratedAuthTokenV2;
            }
            catch (Exception ex) {
                throw new Exception("Error While Generating AuthTokenV2");
            }
        }

        private string GenerateHashSignature(string encodedHeaderJSON, string encodedPayLoadJSON, string apiSecret) {
            string data = encodedHeaderJSON + "." + encodedPayLoadJSON;
            byte[] hash = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(apiSecret)).ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
            return UrlSafe(Convert.ToBase64String(hash));
        }

        private string GenerateEncodedPayloadJSON(string accountCredential, string region) {
            object payLoadObj = new { account_credential = accountCredential,
                region = region,
                type = "AuthTokenV2",
                ts = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
            };
            return ObjectToString(payLoadObj);
        }

        private string UrlSafe(string input) {
            return input.Replace('+', '-').Replace('/', '_');
        }

        private string ObjectToString(object obj)  {
            return UrlSafe(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj))));
        }

        private string GenerateEncodedJSONHeader() {
            object headerObj = new {
                alg = "HS256",
                typ = "JWT"
            };
            return ObjectToString(headerObj);
        }

        internal static TransactionApiRequest SignOut() {
            throw new Exception("SignOut not implemented");
        }
    }
}
