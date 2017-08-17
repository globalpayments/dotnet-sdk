
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.TableService {
    public class LoginResponse : TableServiceResponse {
        public string LocationId { get; set; }
        public string Token { get; set; }
        public string SessionId { get { return "10101"; } }
        public string TableStatus { get; set; }

        public LoginResponse(string json) : base(json) {
            ExpectedAction = "login";
        }

        protected override void MapResponse(JsonDoc response) {
            LocationId = response.GetValue<string>("locID");
            Token = response.GetValue<string>("token");
            TableStatus = response.GetValue<string>("tableStatus");
        }
    }
}
