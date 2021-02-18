using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class ReversalResponse : BillPayResponseBase<Transaction> {
        public override Transaction Map() {
            var authorizationElement = response.Get("a:ReversalTransactionWithReversalAuthorizations");

            return new Transaction() {
                ResponseCode = response.GetValue<string>("a:ResponseCode"),
                ResponseMessage = GetFirstResponseMessage(response),
                ClientTransactionId = authorizationElement.GetValue<string>("a:MerchantTransactionID"),
                Timestamp = authorizationElement.GetValue<string>("a:TransactionDate"),
                TransactionId = authorizationElement.GetValue<string>("a:TransactionID"),
                ReferenceNumber = authorizationElement.GetValue<string>("a:ReferenceTransactionID")
            };
        }
    }
}
