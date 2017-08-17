using System;
using System.Net;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.TableService;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal class TableServiceConnector : Gateway {
        public string LocationId { get; internal set; }
        public string SecurityToken { get; internal set; }
        public string SessionId { get; internal set; }
        public BumpStatusCollection BumpStatusCollection { get; internal set; }

        public bool Configured {
            get {
                return !string.IsNullOrEmpty(LocationId) && !string.IsNullOrEmpty(SecurityToken) && !string.IsNullOrEmpty(SessionId);
            }
        }

        public TableServiceConnector() : base("multipart/form-data;") { }

        public string Call(string endpoint, MultipartForm content) {
            content.Set("locID", LocationId);
            content.Set("token", SecurityToken);
            content.Set("sessionID", SessionId);

            try {
                var response = SendRequest(endpoint, content.Content);
                if (response.StatusCode != HttpStatusCode.OK) {
                    //TODO: add some error handling here
                }
                return response.RawResponse;
            }
            catch (GatewayException) {
                throw;
            }
            catch (Exception exc) {
                throw new GatewayException(exc.Message, exc);
            }
        }
    }
}
