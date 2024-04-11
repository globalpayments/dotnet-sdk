using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GlobalPayments.Api.Tests.FileProcessing {
    public class FileProcessingClient {
        private string UploadUrl { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public FileProcessingClient(string uploadUrl) {
            UploadUrl = uploadUrl;
            if (Headers == null) { Headers = new Dictionary<string, string>(); }

            Headers.Add("Content-Type", "text/csv");
        }

        public bool UploadFile(string filePath) {
            if (!File.Exists(filePath)) {
                throw new Exception("File not found!");
            }
            else if(new FileInfo(filePath).Length > 100000000) {
                throw new Exception("Max file size 100MB exceeded!");
            }

            return SendRequest(HttpMethod.Put, filePath, Headers);
        }

        private bool SendRequest(HttpMethod verb, string filePath, Dictionary<string, string> headers) {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(verb, UploadUrl);           
            HttpResponseMessage response = null;
            try {
                using (var fileStreamContent = new StreamContent(File.OpenRead(filePath))) {
                    //Load the file and set the file's Content-Type header                    
                    fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                    //Add the file                    
                    request.Content = fileStreamContent;
                    //Send it
                    response = httpClient.SendAsync(request).Result;
                }
                
                var rawResponse = response.Content.ReadAsStreamAsync().Result;
                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new Exception();
                }
                return true;
            }
            catch (Exception ex) {
                throw ex;
            }
            finally { }
        }
    }
}
