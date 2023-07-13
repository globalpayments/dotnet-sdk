using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobalPayments.Api.Mapping
{
    public class OpenBankingMapping {
        public static Transaction MapResponse(string rawResponse) {
            Transaction transaction = new Transaction();

            if (!string.IsNullOrEmpty(rawResponse)) {
                JsonDoc json = JsonDoc.Parse(rawResponse);

                transaction.TransactionId = json.Get("order")?.GetValue<string>("id");
                transaction.ClientTransactionId = json.GetValue<string>("ob_trans_id");
                transaction.PaymentMethodType = PaymentMethodType.BankPayment;
                transaction.OrderId = json.Get("order")?.GetValue<string>("id") ?? null;
                transaction.ResponseMessage = json.GetValue<string>("status") ?? null;

                BankPaymentResponse obResponse = new BankPaymentResponse();
                obResponse.RedirectUrl = json.GetValue<string>("redirect_url") ?? null;
                obResponse.PaymentStatus = json.GetValue<string>("status") ?? null;
                obResponse.Id = json.GetValue<string>("ob_trans_id") ?? null;
                obResponse.Amount = json.Get("order")?.GetValue<string>("amount").ToAmount();
                obResponse.Currency = json.Get("order")?.GetValue<string>("currency") ?? null;
                transaction.BankPaymentResponse = obResponse;
            }

            return transaction;
        }

        public static T MapReportResponse<T>(string rawResponse, ReportType reportType) where T : class {
            T result = Activator.CreateInstance<T>();

            JsonDoc json = JsonDoc.Parse(rawResponse);
           
            switch (reportType) {
                case ReportType.FindBankPayment:
                    SetPagingInfo(result as PagedResult<TransactionSummary>, json);
                    foreach (var doc in json.GetArray<JsonDoc>("payments") ?? Enumerable.Empty<JsonDoc>()) {
                        (result as PagedResult<TransactionSummary>).Add(MapTransactionSummary(doc));
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        private static void SetPagingInfo<T>(PagedResult<T> result, JsonDoc json) where T : class {
            result.TotalRecordCount = json.GetValue<int>("total_record_count", "total_count");
            result.PageSize = json.GetValue<int>("max_page_size");
            result.Page = json.GetValue<int>("page_number");            
        }

        public static TransactionSummary MapTransactionSummary(JsonDoc response) {
            TransactionSummary summary = new TransactionSummary();
            summary.TransactionId = response.GetValue<string>("ob_trans_id");
            summary.OrderId = response.GetValue<string>("order_id");
            summary.Amount = response.GetValue<string>("amount").ToAmount();
            summary.Currency = response.GetValue<string>("currency");
            summary.TransactionStatus = response.GetValue<string>("status");
            summary.PaymentType = EnumConverter.GetMapping(Target.GP_ECOM, PaymentMethodName.BankPayment);
            summary.TransactionDate = response.GetValue<DateTime>("created_on");

            BankPaymentResponse bankPaymentData = new BankPaymentResponse();
            bankPaymentData.Id = response.GetValue<string>("ob_trans_id");
            bankPaymentData.Type = GetBankPaymentType(response);
            bankPaymentData.TokenRequestId = response.GetValue<string>("token_request_id");
            bankPaymentData.Iban = response.GetValue<string>("dest_iban");
            bankPaymentData.AccountName = response.GetValue<string>("dest_account_name");
            bankPaymentData.AccountNumber = response.GetValue<string>("dest_account_number");
            bankPaymentData.SortCode = response.GetValue<string>("dest_sort_code");
            bankPaymentData.PaymentStatus = response.GetValue<string>("status");
            summary.BankPaymentResponse = bankPaymentData;

            return summary;
        }

        private static BankPaymentType? GetBankPaymentType(JsonDoc response) {
            if (response.Has("payment_type")) {
                if (BankPaymentType.FASTERPAYMENTS.ToString().ToUpper().Equals(response.GetValue<string>("payment_type").ToUpper())) {
                    return BankPaymentType.FASTERPAYMENTS;
                }
                else if (BankPaymentType.SEPA.ToString().ToUpper().Equals(response.GetValue<string>("payment_type").ToUpper())) {
                    return BankPaymentType.SEPA;
                }
            }
            return null;
        }
    }
}
