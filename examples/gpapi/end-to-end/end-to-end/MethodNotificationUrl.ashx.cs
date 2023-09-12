using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                // TODO: notify client-side that the Method URL step is complete                
              
                context.Response.Write("<script src=\"Scripts/globalpayments-3ds.js\"></script>");
                context.Response.Write("<script>");                
                context.Response.Write(String.Format("GlobalPayments.ThreeDSecure.handleMethodNotification('{0}'); console.log('threeDSServerTransID: {1}');", threeDSServerTransID, threeDSServerTransID));
               
                context.Response.Write("</script>");
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