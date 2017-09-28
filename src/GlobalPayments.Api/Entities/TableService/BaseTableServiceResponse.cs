using System.Collections.Generic;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.TableService
{
    public abstract class BaseTableServiceResponse {
        private List<string> _messageIds;

        /// <summary>
        /// Response code from the table service API
        /// </summary>
        public string ResponseCode { get; set; }
        /// <summary>
        /// Response text from the table service
        /// </summary>
        public string ResponseText { get; set; }
        /// <summary>
        /// type of object being manipulated
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// the type of action being taken on the class
        /// </summary>
        public string Action { get; set; }

        internal BaseTableServiceResponse(string json) {
            if (string.IsNullOrEmpty(json))
                return;

            var response = JsonDoc.Parse(json);

            ResponseCode = NormalizeResponse(response.GetValue<string>("code"));
            ResponseText = response.GetValue<string>("codeMsg");
            Class = response.GetValue<string>("class");
            Action = response.GetValue<string>("action");

            if (!ResponseCode.Equals("00")) {
                throw new MessageException(ResponseText);
            }

            // map response from the data rows
            if (response.Has("data")) {
                var data = response.Get("data");
                if (data.Has("row")) {
                    var row = data.Get("row");
                    MapResponse(row ?? data);
                }
            }
        }

        protected abstract void MapResponse(JsonDoc response);

        protected string NormalizeResponse(string responseCode) {
            var acceptedCodes = new List<string>() { "01" };
            if (acceptedCodes.Contains(responseCode))
                return "00";
            return responseCode;
        }
    }
}
