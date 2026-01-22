using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace end_to_end
{
    /// <summary>
    /// Summary description for MethodNotificationUrl
    /// </summary>
    public class MethodNotificationUrl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            // context.Response.ContentType = "text/plain";
            // context.Response.Write("Hello World");
            /*
            * this sample code is intended as a simple example and should not be treated as Production-ready code 
            * you'll need to add your own message parsing and security in line with your application or website
            */
            var threeDSMethodData = context.Request.Form["threeDSMethodData"];

            // sample ACS response for Method URL Response Notification
            // threeDSMethodData = "eyJ0aHJlZURTU2VydmVyVHJhbnNJRCI6ImFmNjVjMzY5LTU5YjktNGY4ZC1iMmY2LTdkN2Q1ZjVjNjlkNSJ9";

            try
            {
                byte[] data = Convert.FromBase64String(threeDSMethodData);
                string methodUrlResponseString = Encoding.UTF8.GetString(data);

                // map to a custom class MethodUrlResponse
                MethodUrlResponse methodUrlResponse = JsonConvert.DeserializeObject<MethodUrlResponse>(methodUrlResponseString);

                string threeDSServerTransID = methodUrlResponse.ThreeDSServerTransID; // af65c369-59b9-4f8d-b2f6-7d7d5f5c69d5

                // Validate the transaction ID format (UUID format)
                if (string.IsNullOrWhiteSpace(threeDSServerTransID) || 
                    !Regex.IsMatch(threeDSServerTransID, @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")) {
                    context.Response.StatusCode = 400;
                    return;
                }

                // TODO: notify client-side that the Method URL step is complete    

                // Properly serialize to JSON to prevent XSS
                string serverTransIdJson = JsonConvert.SerializeObject(threeDSServerTransID);

                context.Response.ContentType = "text/html";
                using (var writer = new System.IO.StreamWriter(context.Response.OutputStream, Encoding.UTF8)) {
                    writer.Write("<script src=\"Scripts/globalpayments-3ds.js\"></script>");
                    writer.Write("<script>");
                    writer.Write("GlobalPayments.ThreeDSecure.handleMethodNotification(" + serverTransIdJson + "); ");
                    writer.Write("console.log('threeDSServerTransID: ' + " + serverTransIdJson + ");");
                    writer.Write("</script>");
                }
            }

            catch (Exception exce)
            {
                // TODO: add your exception handling here
            }
        }

        public class MethodUrlResponse
        {
            public string ThreeDSServerTransID { get; set; }
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