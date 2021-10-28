using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Payroll {
    internal class SessionInfo : PayrollEntity {
        public string SessionToken { get; private set; }
        public string ErrorMessage { get; private set; }

        internal override void FromJson(JsonDoc doc, PayrollEncoder encoder) {
            SessionToken = doc.GetValue<string>("SessionToken");
            ErrorMessage = doc.GetValue<string>("ErrorMessage");
        }

        internal static PayrollRequest SignIn(string username, string password, PayrollEncoder encoder) {
            var request = new JsonDoc()
                .Set("Username", username)
                .Set("Password", encoder.Encode(password));

            return new PayrollRequest {
                Endpoint = "/api/pos/session/signin",
                RequestBody = request.ToString()
            };
        }

        internal static PayrollRequest SignOut() {
            return new PayrollRequest {
                Endpoint = "/api/pos/session/signout"
            };
        }
    }
}
