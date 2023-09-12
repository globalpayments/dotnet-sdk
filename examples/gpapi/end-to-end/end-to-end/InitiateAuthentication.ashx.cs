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
    /// Summary description for InitiateAuthentication
    /// </summary>
    public class InitiateAuthentication : IHttpHandler
    {
        public class ChallengeWindow
        {
            public string windowSize { get; set; }
            public string displayMode { get; set; }
        }
        public class InitiateAuth
        {
            public string authenticationRequestType { get; set; }
            public string serverTransactionId { get; set; }
            public bool methodUrlComplete { get; set; }
            public string tokenResponse { get; set; }
            public string authenticationSource { get; set; }
            public ChallengeWindow challengeWindow { get; set; }
            public Order order { get; set; }
            public BrowserData browserData { get; set; }
        }

        public class Order
        {
            public string currency { get; set; }
            public decimal amount { get; set; }
        }

        public class BrowserData
        {
            public string colorDepth { get; set; }
            public bool javaEnabled { get; set; }
            public bool javascriptEnabled { get; set; }
            public string language { get; set; }
            public int screenHeight { get; set; }
            public int screenWidth { get; set; }
            public DateTime time { get; set; }
            public int timezoneOffset { get; set; }
            public string userAgent { get; set; }
        }



        public void ProcessRequest(HttpContext context)
        {
            Stream stream = context.Request.InputStream;
            StreamReader sr = new StreamReader(stream);

            string baseUrl = context.Request.UrlReferrer.AbsoluteUri;            
            string currentIP = baseUrl;

            string json = sr.ReadToEnd();

            InitiateAuth requestData = JsonConvert.DeserializeObject<InitiateAuth>(json);

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
            //config.RequestLogger = new RequestFileLogger(@"C:\temp\gpapi\initiate.txt");

            ServicesContainer.ConfigureService(config);

            Address billingAddress = new Address();
            billingAddress.StreetAddress1 = "Apartment 852";
            billingAddress.StreetAddress2 = "Complex 741";
            billingAddress.StreetAddress3 = "Unit 4";
            billingAddress.City = "Chicago";
            billingAddress.State = "IL";
            billingAddress.PostalCode = "50001";
            billingAddress.CountryCode = "840";

            Address shippingAddress = new Address();
            shippingAddress.StreetAddress1 = "Flat 456";
            shippingAddress.StreetAddress2 = "House 789";
            shippingAddress.StreetAddress3 = "Basement Flat";
            shippingAddress.City = "Halifax";
            shippingAddress.PostalCode = "W5 9HR";
            shippingAddress.CountryCode = "826";

            // TODO: Add captured browser data from the client-side and server-side
            GlobalPayments.Api.Entities.BrowserData browserData = new GlobalPayments.Api.Entities.BrowserData();
            browserData.AcceptHeader = context.Request.ServerVariables["HTTP_ACCEPT"];
            browserData.ColorDepth = (ColorDepth)Enum.Parse(typeof(ColorDepth), requestData.browserData.colorDepth);
            browserData.IpAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            browserData.JavaEnabled = requestData.browserData.javaEnabled;
            browserData.JavaScriptEnabled = requestData.browserData.javascriptEnabled;
            browserData.Language = requestData.browserData.language;
            browserData.ScreenHeight = requestData.browserData.screenHeight;
            browserData.ScreenWidth = requestData.browserData.screenWidth;
            browserData.ChallengeWindowSize = (ChallengeWindowSize)Enum.Parse(typeof(ChallengeWindowSize), requestData.challengeWindow.windowSize);
            browserData.Timezone = requestData.browserData.timezoneOffset.ToString();
            browserData.UserAgent = requestData.browserData.userAgent;

            CreditCardData paymentMethod = new CreditCardData();
            paymentMethod.Token = requestData.tokenResponse;
            paymentMethod.CardHolderName = "James Mason";

            ThreeDSecure threeDSecureData = new ThreeDSecure();
            threeDSecureData.ServerTransactionId = requestData.serverTransactionId;
            var methodUrlCompletion = MethodUrlCompletion.YES;
            try
            {
                threeDSecureData = Secure3dService.InitiateAuthentication(paymentMethod, threeDSecureData)
                    .WithAmount(requestData.order.amount)
                    .WithCurrency(requestData.order.currency)
                    .WithOrderCreateDate(DateTime.Now)
                    .WithAddress(billingAddress, AddressType.Billing)
                    .WithAddress(shippingAddress, AddressType.Shipping)
                    .WithAddressMatchIndicator(false)
                    .WithAuthenticationSource((AuthenticationSource)Enum.Parse(typeof(AuthenticationSource), requestData.authenticationSource))
                    .WithBrowserData(browserData)
                    .WithMethodUrlCompletion(methodUrlCompletion)
                    .Execute();
            }
            catch (ApiException e)
            {
                // TODO: add your error handling here
                context.AddError(e);
            }

            var status = threeDSecureData.Status;

            string response;

            response = JsonConvert.SerializeObject(new
            {
                liabilityShift = threeDSecureData.LiabilityShift,
            });


            if (status != Secure3dStatus.CHALLENGE_REQUIRED.ToString())
            {
                // Frictionless flow
                response = JsonConvert.SerializeObject(new
                {
                    liabilityShift = threeDSecureData.LiabilityShift,
                    result = threeDSecureData.Status,
                    authenticationValue = threeDSecureData.AuthenticationValue,
                    serverTransactionId = threeDSecureData.ServerTransactionId,
                    messageVersion = threeDSecureData.MessageVersion,
                    eci = threeDSecureData.Eci
                });

            }
            else
            {
                //Challenge flow
                response = JsonConvert.SerializeObject(new
                {
                    liabilityShift = threeDSecureData.LiabilityShift,
                    status = threeDSecureData.Status, // CHALLENGE_REQUIRED
                    challengeMandated = threeDSecureData.ChallengeMandated,
                    challenge = new Dictionary<string, string>
                    {
                        { "requestUrl", threeDSecureData.IssuerAcsUrl },
                        { "encodedChallengeRequest", threeDSecureData.PayerAuthenticationRequest },
                        { "messageType", threeDSecureData.MessageType}
                    },
                });
            }


            context.Response.ContentType = "application/json";
            context.Response.Write(response);
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