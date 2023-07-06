using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace end_to_end
{
    /// <summary>
    /// Summary description for ChallengeNotificationUrl
    /// </summary>
    public class ChallengeNotificationUrl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            /*
          * this sample code is intended as a simple example and should not be treated as Production-ready code 
          * you'll need to add your own message parsing and security in line with your application or website
          */
            var cres = context.Request.Form["cres"];
            string challengeUrlResponseString = string.Empty;
            string request = string.Empty;
            string threeDSServerTransID = string.Empty;
            string transStatus = string.Empty;
            try
            {               
                var a = Convert.FromBase64String(cres.Replace('-', '+').Replace('_', '/') + "=");
                challengeUrlResponseString = Encoding.UTF8.GetString(a);

                ChallengeUrlResponse challengeUrlResponse = JsonConvert.DeserializeObject<ChallengeUrlResponse>(challengeUrlResponseString);

                 threeDSServerTransID = challengeUrlResponse.ThreeDSServerTransID; // af65c369-59b9-4f8d-b2f6-7d7d5f5c69d5
                var  acsTransId = challengeUrlResponse.AcsTransID; // 13c701a3-5a88-4c45-89e9-ef65e50a8bf9
                var messageType = challengeUrlResponse.MessageType; // Cres
                var messageVersion = challengeUrlResponse.MessageVersion; // 2.1.0
                //var challengeCompletionInd = challengeUrlResponse.ChallengeCompletionInd;
                transStatus = challengeUrlResponse.TransStatus; // Y

                // TODO: notify client-side that the Challenge step is complete and pass any required data

                StringBuilder stringOut = new StringBuilder();
                stringOut.AppendLine("{\"threeDSServerTransID\":\"" + threeDSServerTransID + "\"" + ",\"transStatus\":\"" + transStatus + "\"}");                

                //string whattheheckgenererated = stringOut.ToString();

                request = stringOut.ToString();               

                context.Response.Write("<script src=\"Scripts/globalpayments-3ds.js\"></script>");
                context.Response.Write("<script>");
                context.Response.Write(String.Format("GlobalPayments.ThreeDSecure.handleChallengeNotification({0});", stringOut));
                context.Response.Write("</script>");
                
            }
            catch (Exception e)
            {
                
                context.Response.Write(String.Format("console.log('Error Challenge : , {0});", request));
                context.Response.Write("</script>");
            }
            
        }

        public class ChallengeUrlResponse
        {
            [JsonProperty("threeDSServerTransID")]
            public string ThreeDSServerTransID { get; set; }

            [JsonProperty("acsTransID")]
            public string AcsTransID { get; set; }

            [JsonProperty("messageType")]
            public string MessageType { get; set; }

            [JsonProperty("messageVersion")]
            public string MessageVersion { get; set; }          

            [JsonProperty("transStatus")]
            public string TransStatus { get; set; }

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