using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace end_to_end
{
    /// <summary>
    /// Summary description for Authorization
    /// </summary>
    public class Authorization : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var serverTransactionId = context.Request.Form["serverTransactionId"];
            var paymentToken = context.Request.Form["tokenResponse"];

            string baseUrl = context.Request.UrlReferrer.AbsoluteUri;            
            string currentIP = baseUrl;

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

            ServicesContainer.ConfigureService(config);

            ThreeDSecure secureEcom = new ThreeDSecure();

            try
            {
                secureEcom = Secure3dService.GetAuthenticationData()
                   .WithServerTransactionId(serverTransactionId)
                   .Execute();
            }

            catch (ApiException exce)
            {
                // TODO: add your error handling here
            }

            var authenticationValue = secureEcom.AuthenticationValue;
            var dsTransId = secureEcom.DirectoryServerTransactionId;
            var messageVersion = secureEcom.MessageVersion;
            var eci = secureEcom.Eci;

            context.Response.ContentType = "text/html";

            context.Response.Write("<head>");
            context.Response.Write("<title>3D Secure 2 Authentication</title>");
            context.Response.Write("</head>");

            context.Response.Write("<body>");
            context.Response.Write("<h2>3D Secure 2 Authentication</h2>");
            context.Response.Write("<p><strong>Hurray! Your trasaction was authenticated successfully!</strong></p>");
            context.Response.Write($"<p>Server Trans ID: {serverTransactionId}</p>");
            context.Response.Write($"<p>Authentication Value: {authenticationValue}</p>");
            context.Response.Write($"<p>DS Trans ID: {dsTransId}</p>");
            context.Response.Write($"<p>Message Version: {messageVersion}</p>");
            context.Response.Write($"<p>ECI: {eci}</p>");

            context.Response.Write($"<hr>");

            context.Response.Write($"<h2>Transaction details:</h2>");


            CreditCardData paymentMethod = new CreditCardData();
            paymentMethod.Token = paymentToken;
            paymentMethod.ThreeDSecure = secureEcom;
            Transaction response = null;

            // proceed to authorization with liability shift
            try
            {
                response = paymentMethod.Charge(100)
                   .WithCurrency("EUR")
                   .Execute();
            }
            catch (Exception e)
            {
                // TODO: Add your error handling here
                context.Response.Write($"<p> Error: {e.Message}</p>");

            }
            if (response != null)
            {
                var transactionId = response.TransactionId;
                var transactionStatus = response.ResponseMessage;

                context.Response.Write($"<p> Trans ID: {transactionId}</p>");
                context.Response.Write($"<p> Trans status: {transactionStatus}</p>");
            }

            context.Response.Write("</body>");
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