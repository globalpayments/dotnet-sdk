using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace end_to_end
{
    /// <summary>
    /// Summary description for Check3dsVersion
    /// </summary>
    public class Check3dsVersion : IHttpHandler
    {
        public class TokenResponse
        {
            public string token { get; set; }
        }

        public class RequestJson { 
        public string tokenResponse { get; set; }
        }

        public void ProcessRequest(HttpContext context)
        { 
            Stream stream = context.Request.InputStream;
            StreamReader sr = new StreamReader(stream);
            string baseUrl = context.Request.UrlReferrer.AbsoluteUri;           
            string currentIP = baseUrl;

            string json = sr.ReadToEnd();

           
            RequestJson paymentToken = JsonConvert.DeserializeObject<RequestJson>(json);
           
            // configure client & request settings
            GpApiConfig config = new GpApiConfig();
            config.AppId = _Default.APP_ID;
            config.AppKey = _Default.APP_KEY;
            config.Environment = GlobalPayments.Api.Entities.Environment.TEST;
            config.Country = "GB";
            config.Channel = Channel.CardNotPresent;
            config.MethodNotificationUrl = string.Format("{0}MethodNotificationUrl.ashx", currentIP);
            config.MerchantContactUrl = "https://www.example.com/about";
            config.ChallengeNotificationUrl = string.Format("{0}ChallengeNotificationUrl.ashx", currentIP);
            //config.RequestLogger = new RequestFileLogger(@"C:\temp\gpapi\checkversion.txt");

            ServicesContainer.ConfigureService(config);

            CreditCardData card = new CreditCardData();
            card.Token = paymentToken.tokenResponse;

            ThreeDSecure threeDSecureData = new ThreeDSecure();

            try
            {
                 threeDSecureData = Secure3dService.CheckEnrollment(card)
                  .WithCurrency("EUR")
                  .WithAmount(100)
                  .Execute();
            }
            catch (ApiException e) {
                // TODO: add your error handling here
                context.AddError(e);                
            }

            var enrolled = threeDSecureData.Enrolled; // ENROLLED
            // if enrolled, the available response data
            var serverTransactionId = threeDSecureData.ServerTransactionId;
            var messageVersion = threeDSecureData.Version;
            var methodUrl = threeDSecureData.IssuerAcsUrl;
            var encodedMethodData = threeDSecureData.PayerAuthenticationRequest; // Base64 encoded string
            var payerAuthenticationRequest = threeDSecureData.PayerAuthenticationRequest;
            var issuerAcsUrl = threeDSecureData.IssuerAcsUrl;

            // simple example of how to prepare the JSON string for JavaScript Library
            string responseJson;

            responseJson = JsonConvert.SerializeObject(new
            {
                enrolled = enrolled,
                messageVersion = messageVersion
            }); 

            if (enrolled == Secure3dStatus.ENROLLED.ToString() && messageVersion == Secure3dVersion.Two) {
                responseJson = JsonConvert.SerializeObject(new
                {
                    enrolled = enrolled,
                    messageVersion = messageVersion,
                    serverTransactionId = serverTransactionId,
                    methodUrl = methodUrl,
                    methodData = encodedMethodData
                });    
            }

            context.Response.ContentType = "application/json";
            context.Response.Write(responseJson);
            context.Response.Flush();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}