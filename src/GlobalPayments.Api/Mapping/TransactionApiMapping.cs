using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Mapping {
    public class TransactionApiMapping {
        public static T MapReportResponse<T>(string response, ReportType reportType) where T : class {
            throw new NotImplementedException();
        }

        public static Transaction MapResponse(string rawResponse, TransactionType transType, TransactionType originalTransactionType = TransactionType.Sale) {
            Transaction transaction = new Transaction();
            if (!string.IsNullOrEmpty(rawResponse)) {
                JsonDoc json = JsonDoc.Parse(rawResponse);

                transaction.ReferenceNumber = json.GetValue<string>("reference_id");
                string transId = "";
                switch (transType) {
                    case TransactionType.Auth:
                        transId = "creditauth_id";
                        break;
                    case TransactionType.Fetch:
                    case TransactionType.Void:
                        if (originalTransactionType == TransactionType.Sale)
                            transId = json.Has("card") ? "creditsale_id" : "checksale_id";
                        else
                            transId = json.Has("card") ? "creditreturn_id" : "checkrefund_id";
                        break;
                    case TransactionType.Sale:
                        transId = json.Has("card") ? "creditsale_id" : "checksale_id";
                        break;
                    case TransactionType.Edit:
                        transId = json.Has("card") ? "creditsale_id" : "checksale_id";
                        break;
                    case TransactionType.Refund:
                        transId = json.Has("card") ? "creditreturn_id" : "checkrefund_id";
                        break;
                    default:
                        break;
                };

                transaction.TransactionId = json.GetValue<String>(transId);
                transaction.ResponseCode = json.GetValue<string>("status");
                transaction.AuthorizationCode = json.GetValue<string>("approval_code");
                transaction.AvsResponseCode = json.GetValue<string>("avs_response");
                transaction.AvsResponseMessage = json.GetValue<string>("avs_response_description");
                transaction.CvnResponseMessage = json.GetValue<string>("cardsecurity_response");
                transaction.ResponseMessage = json.GetValue<string>("processor_response");
                transaction.ReferenceNumber = json.GetValue<string>("reference_id");
                if (json.Has("card")) {
                    transaction.CardDetail = new Entities.TransactionApi.Response.CardResponse
                    {
                        MaskedCardNnumber = json.Get("card").GetValue<string>("masked_card_number"),
                        CardholderName = json.Get("card").GetValue<string>("cardholder_name"),
                        ExpiryMonth = json.Get("card").GetValue<string>("expiry_month"),
                        ExpiryYear = json.Get("card").GetValue<string>("expiry_year"),
                        Type = json.Get("card").GetValue<string>("type"),
                        Token = json.Get("card").GetValue<string>("token"),
                        Balance = json.Get("card").GetValue<string>("balance")
                    };
                }
                if (json.Has("check")) {
                    transaction.CheckDetail = new Entities.TransactionApi.Response.CheckResponse {
                        MaskedCardNnumber = json.Get("check").GetValue<string>("masked_account_number"),
                        RoutingNumber = json.Get("check").GetValue<string>("routing_number"),
                        CheckNumber = json.Get("check").GetValue<string>("check_number"),
                        Token = json.Get("check").GetValue<string>("token")
                    };
                }

                transaction.PaymentDetail = new Entities.TransactionApi.Response.PaymentResponse {
                    Amount = json.Get("payment").GetValue<string>("amount"),
                    CurrencyCode = json.Get("payment").GetValue<string>("currency_code"),
                    InvoiceNumber = json.Get("payment").GetValue<string>("invoice_number"),
                    GratuityAmount = json.Get("payment").GetValue<string>("gratuity_amount"),
                    Type = json.Get("payment").GetValue<string>("type")
                };

                transaction.TransDetail = new Entities.TransactionApi.Response.TransactionResponseDetail {
                    BatchAmount = json.Get("transaction").GetValue<string>("batch_amount"),
                    BatchNumber = json.Get("transaction").GetValue<string>("batch_number"),
                    EntryType = json.Get("transaction").GetValue<string>("entry_type"),
                    Language = json.Get("transaction").GetValue<string>("language"),
                    EntryClass = json.Get("transaction").GetValue<string>("entry_class")
                };
            }
            return transaction;
        }
    }
}