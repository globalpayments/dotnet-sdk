
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.TableService {
    public class LoginResponse : TableServiceResponse {
        /// <summary>
        /// Location Id of the restauant
        /// </summary>
        public string LocationId { get; set; }
        /// <summary>
        /// security token for subsequent calls
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Session Id (should always be 10101)
        /// </summary>
        public string SessionId { get { return "10101"; } }
        /// <summary>
        /// status string as returned from the table service API
        /// </summary>
        public string TableStatus { get; set; }

        public LoginResponse(string json, string configName = "default") : base(json, configName) {
            ExpectedAction = "login";
        }

        protected override void MapResponse(JsonDoc response) {
            LocationId = response.GetValue<string>("locID");
            Token = response.GetValue<string>("token");
            TableStatus = response.GetValue<string>("tableStatus");
        }
    }
}
