using GlobalPayments.Api.Entities.Billing;

namespace GlobalPayments.Api.Gateways.BillPay {
	internal sealed class ConvenienceFeeRequestResponse : BillPayResponseBase<ConvenienceFeeResponse> {
		public override ConvenienceFeeResponse Map() {
			return new ConvenienceFeeResponse() {
				IsSuccessful = response.GetValue<bool>("a:isSuccessful"),
				ResponseCode = response.GetValue<string>("a:ResponseCode"),
				ResponseMessage = GetFirstResponseMessage(response),
				ConvenienceFee = response.GetValue<decimal>("a:ConvenienceFee")
			};
		}
	}
}
