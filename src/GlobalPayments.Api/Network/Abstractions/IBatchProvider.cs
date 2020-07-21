using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Network.Abstractions {
    public interface IBatchProvider {
        int GetBatchNumber();
        int GetSequenceNumber();
        int GetTransactionCount();
        decimal GetTotalCredits();
        decimal GetTotalDebits();
        IRequestEncoder GetRequestEncoder();
        List<string> GetEncodedRequests();
        PriorMessageInformation GetPriorMessageData();
        void SetPriorMessageData(PriorMessageInformation value);
        void ReportDataCollect(TransactionType transactionType, PaymentMethodType paymentMethodType, decimal amount, string encodedRequest);
        void CloseBatch(bool inBalance);

        int ProprietaryTransactionCount { get; set; }
        decimal ProprietaryTransactionSales { get; set; }
        int VisaTransactionCount { get; set; }
        decimal VisaTransactionSales { get; set; }
        int MasterCardTransactionCount { get; set; }
        decimal MasterCardTransactionSales { get; set; }
        int OtherCardTransactionCount { get; set; }
        decimal OtherCardTransactionSales { get; set; }
        int CreditCardTransactionCount { get; set; }
        decimal CreditCardTransactionSales { get; set; }
        int DebitEBTTransactionCount { get; set; }
        decimal DebitEBTTransactionSales { get; set; }
        int CreditVoidTransactionCount { get; set; }
        decimal CreditVoidTransactionSales { get; set; }
        int DebitEBTVoidTransactionCount { get; set; }
        decimal DebitEBTVoidTransactionSales { get; set; }
        int CreditReturnTransactionCount { get; set; }
        decimal CreditReturnTransactionSales { get; set; }
        int DebitReturnTransactionCount { get; set; }
        decimal DebitReturnTransactionSales { get; set; }
        string CardType { get; set; }
        int TotalTransactionCount { get; set; }
    }
}
