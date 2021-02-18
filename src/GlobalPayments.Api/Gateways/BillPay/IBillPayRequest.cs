using GlobalPayments.Api.Entities.Billing;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal interface IBillPayRequest<TBuilder, TResponse> {
        IBillPayRequest<TBuilder, TResponse> Build(TBuilder builder, Credentials credentials);
        IBillPayResponse<TResponse> Execute(string endpoint = "");
    }
}
