using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Genius.Request;
using GlobalPayments.Api.Terminals.Genius.Responses;
using GlobalPayments.Api.Utils;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace GlobalPayments.Api.Terminals.Genius.Interfaces
{
    internal class MitcGateway : RestGateway
    {
        private static readonly Encoding UTF8_CHARSET = Encoding.UTF8;
        public ConnectionConfig Config { get; }
        private readonly string accountCredential;
        private static readonly string TRANSACTION_API_VERSION = "2021-04-08";
        private string _requestId;
        public string TargetDevice;

        public MitcGateway(ConnectionConfig settings) 
        {
            Config = settings;
            TargetDevice = Config.GeniusMitcConfig.TargetDevice;
            _requestId = Config.GeniusMitcConfig.RequestId;
            ServiceUrl = Config.Environment == Entities.Environment.PRODUCTION ? ServiceEndpoints.GENIUS_MITC_PRODUCTION : ServiceEndpoints.GENIUS_MITC_TEST;
                        
            accountCredential =
                $"{Config.GeniusMitcConfig.xWebId}:{Config.GeniusMitcConfig.TerminalId}:{Config.GeniusMitcConfig.AuthKey}";

            Headers["X-GP-Version"] = TRANSACTION_API_VERSION;
            Headers["X-GP-Api-Key"] = Config.GeniusMitcConfig.ApiKey;
            Headers["X-GP-Target-Device"] = Config.GeniusMitcConfig.TargetDevice;
            Headers["X-GP-Partner-App-Name"] = Config.GeniusMitcConfig.AppName;
            Headers["X-GP-Partner-App-Version"] = Config.GeniusMitcConfig.AppVersion;
        }
        public MitcResponse DoTransaction(GeniusMitcRequest.HttpMethod verb, string endpoint, string requestBody, MitcRequestType requestType)
        {
            try
            {
                Headers["Authorization"] = "AuthToken " + GenerateToken();
                Headers["X-GP-Request-Id"] = _requestId;
                Headers["X-GP_Version"] = "2021-04-08";
                Timeout = 30000;
                var response = SendRequest(verb.GetVerb(), endpoint, requestBody, null, null, false);
                return new MitcResponse((int)response.StatusCode, "", response.RawResponse);
            }
            catch (GatewayException ex)
            {
                // Handling error response messages
                if (ex.ResponseCode != null &&
                    (ex.ResponseCode.Equals("471") ||
                     ex.ResponseCode.Equals("470") ||
                     ex.ResponseCode.Equals("404") ||
                     ex.ResponseCode.Equals("400") ||
                     ex.ResponseCode.Equals("403") ||
                     ex.ResponseCode.Equals("401")))
                {                    
                    JsonDoc responseObj = JsonDoc.Parse(ex.ResponseMessage);                   
                }
                throw ex;
            }
            catch (Exception ex)
            {
                throw new GatewayException(ex.Message);
            }
        }

        #region Private methods for AuthToken
        private string GenerateToken()
        {           
            try
            {
                string region = Config.GeniusMitcConfig.Region;
                string apiSecret = Config.GeniusMitcConfig.ApiSecret;

                string encodedHeaderJSON = generateEncodedJSONHeader();
                string encodedPayLoadJSON = generateEncodedPayloadJSON(this.accountCredential, region);
                string encodedSignature = generateHashSignature(encodedHeaderJSON, encodedPayLoadJSON, apiSecret);

                /**
                * Create the JWT AuthToken by concatenating Base64 URL encoded header,
                * payload and signature
                */               
                string GeneratedAuthTokenV2 = encodedHeaderJSON + "." + encodedPayLoadJSON + "." + encodedSignature;
                return GeneratedAuthTokenV2;
            }
            catch (Exception err)
            {
                throw new GatewayException(err.Message, err);               
            }
        }

        private string generateHashSignature(string encodedHeaderJSON, string encodedPayLoadJSON, string apiSecret)
        {
            string data = encodedHeaderJSON + "." + encodedPayLoadJSON;
            byte[] hash = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(apiSecret)).ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
            return UrlSafe(Convert.ToBase64String(hash));
        }
        private string UrlSafe(string input)
        {
            return input.Replace('+', '-').Replace('/', '_');
        }
        private string objectToString(object obj)
        {
            return UrlSafe(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj))));
        }
        private string generateEncodedPayloadJSON(string accountCredential, string region)
        {
            object payLoadObj = new
            {
                type = "AuthTokenV2",
                account_credential = accountCredential,
                region = region,               
                ts = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
            };
            return objectToString(payLoadObj);
        }
        private string generateEncodedJSONHeader()
        {
            object headerObj = new
            {
                alg = "HS256",
                typ = "JWT"
            };
            return objectToString(headerObj);
        }
        #endregion
    }
}
