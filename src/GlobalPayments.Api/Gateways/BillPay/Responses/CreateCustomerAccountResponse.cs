using GlobalPayments.Api.Entities.Billing;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class CreateCustomerAccountResponse : BillPayResponseBase<TokenResponse> {
        public override TokenResponse Map() {
            return new TokenResponse {
                IsSuccessful = response.GetValue<bool>("a:isSuccessful"),
                ResponseCode = response.GetValue<string>("a:ResponseCode"),
                ResponseMessage = GetFirstResponseMessage(response),
                Token = response.GetValue<string>("a:Token")
            };
        }
    }
}
