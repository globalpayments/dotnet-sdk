using System;
using System.Collections.Generic;
using System.Text;
using static GlobalPayments.Api.Terminals.Genius.Request.GeniusMitcRequest;

namespace GlobalPayments.Api.Terminals.Genius.Request
{
    public class GeniusMitcRequest
    {
        public HttpMethod Verb { get; set; } = HttpMethod.GET;
        public string Endpoint { get; set; }
        public string RequestBody { get; set; } = "";
        public Dictionary<string, string> QueryStringParams { get; }
        public GeniusMitcRequest()
        {
            QueryStringParams = new Dictionary<string, string>();
        }

        public enum HttpMethod
        {
            GET,
            POST,
            PUT
        }   
    }
    public static class GeniusMitcRequestExtensions
    {
        public static System.Net.Http.HttpMethod GetVerb(this HttpMethod httpMethod)
        {
            switch (httpMethod)
            {
                case HttpMethod.POST:
                    return System.Net.Http.HttpMethod.Post;
                case HttpMethod.PUT:
                    return System.Net.Http.HttpMethod.Put;
                case HttpMethod.GET:
                    return System.Net.Http.HttpMethod.Get;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
