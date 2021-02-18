using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class TransactionResponse : BillPayResponseBase<Transaction> {
        public override Transaction Map() {
            return new Transaction() {
                ResponseCode = response.GetValue<string>("a:ResponseCode"),
                ResponseMessage = GetFirstResponseMessage(response),
                AvsResponseCode = response.GetValue<string>("a:AvsResponseCode"),
                AvsResponseMessage = response.GetValue<string>("a:AvsResponseText"),
                CvnResponseCode = response.GetValue<string>("a:CvvResponseCode"),
                CvnResponseMessage = response.GetValue<string>("a:CvvResponseText"),
                ClientTransactionId = response.GetValue<string>("a:MerchantTransactionID"),
                Timestamp = response.GetValue<string>("a:TransactionDate"),
                TransactionId = response.GetValue<string>("a:Transaction_ID"),
                ReferenceNumber = response.GetValue<string>("a:ReferenceTransactionID"),
                Token = response.GetValue<string>("a:Token"),
                ConvenienceFee = response.GetValue<decimal>("a:ConvenienceFee")
            };
        }
    }
}
