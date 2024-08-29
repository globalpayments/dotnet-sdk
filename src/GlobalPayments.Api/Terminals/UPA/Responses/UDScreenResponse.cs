using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.UPA.Responses {
    public class UDScreenResponse : UPAResponseHandler, IDeviceScreen {
        public UDScreenResponse(JsonDoc root) {
            ParseResponse(root);
        }
    }
}
