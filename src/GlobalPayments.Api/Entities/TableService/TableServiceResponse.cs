using System;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.TableService {
    public class TableServiceResponse : BaseTableServiceResponse {
        protected string _configName = "default";
        protected string ExpectedAction { get; set; }

        public TableServiceResponse(string json, string configName = "default") : base(json) {
            _configName = configName;
        }

        protected override void MapResponse(JsonDoc response) {
            if(!string.IsNullOrEmpty(ExpectedAction) && !Action.Equals(ExpectedAction))
                throw new MessageException(string.Format("Unexpected message type received. {0}.", Action));
        }

        protected T SendRequest<T>(string endpoint, MultipartForm formData) where T : TableServiceResponse {
            var connector = ServicesContainer.Instance.GetTableServiceClient(_configName);
            if (!connector.Configured && !endpoint.Equals("user/login"))
                throw new ConfigurationException("Reservation service has not been configured properly. Please ensure you have logged in first.");

            var response = connector.Call(endpoint, formData);
            return Activator.CreateInstance(typeof(T), response, _configName) as T;
        }
    }
}
