namespace GlobalPayments.Api.Gateways.BillPay {
    internal interface IBillPayResponse<T> {
        T Map();
        IBillPayResponse<T> WithResponseTagName(string tagName);
        IBillPayResponse<T> WithResponse(string response);
    }
}
