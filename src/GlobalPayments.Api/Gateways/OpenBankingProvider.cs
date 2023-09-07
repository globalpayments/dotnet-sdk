using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.Mapping;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Gateways
{
    internal class OpenBankingProvider : RestGateway, IOpenBankingProvider, IReportingService {
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string SharedSecret { get; set; }
        public bool SupportsHostedPayments => false;
        public ShaHashType ShaHashType { get; set; }

        private Dictionary<string, string> MaskedValues;

        public OpenBankingProvider() {
            this.Headers["Accept"] = "application/json";
        }

        public Transaction ProcessOpenBanking(AuthorizationBuilder builder) {
            string timestamp = builder.Timestamp ?? GenerationUtils.GenerateTimestamp(); 
            string orderId = builder.OrderId ?? GenerationUtils.GenerateOrderId();
            var amount = builder.Amount != null ? builder.Amount.ToNumericCurrencyString() : null;

            JsonDoc request = new JsonDoc();

            var paymentMethod = builder.PaymentMethod as BankPayment;
            switch (builder.TransactionType) {
                case TransactionType.Sale:
                    var bankPaymentType = paymentMethod.BankPaymentType != null ?
                            paymentMethod.BankPaymentType : GetBankPaymentType(builder.Currency);
                    string hash = GenerationUtils.GenerateHash(SharedSecret, ShaHashType, timestamp, MerchantId, orderId, amount, builder.Currency,
                             !string.IsNullOrEmpty(paymentMethod.SortCode) && bankPaymentType.Equals(BankPaymentType.FASTERPAYMENTS) ?
                                 paymentMethod.SortCode : "",
                             !string.IsNullOrEmpty(paymentMethod.AccountNumber) && bankPaymentType.Equals(BankPaymentType.FASTERPAYMENTS) ?
                                 paymentMethod.AccountNumber : "",
                             !string.IsNullOrEmpty(paymentMethod.Iban) && bankPaymentType.Equals(BankPaymentType.SEPA) ? paymentMethod.Iban : "");
                    SetAuthorizationHeader(hash);

                    request.Set("request_timestamp", timestamp)
                           .Set("merchant_id", MerchantId)
                           .Set("account_id", AccountId);

                    JsonDoc order = new JsonDoc();
                    order.Set("id", orderId)
                          .Set("currency", builder.Currency)
                          .Set("amount", amount)
                          .Set("description", builder.Description);

                    JsonDoc payment = new JsonDoc();

                    JsonDoc destination = new JsonDoc();
                    destination.Set("account_number", bankPaymentType.Equals(BankPaymentType.FASTERPAYMENTS) ? paymentMethod.AccountNumber : null)
                          .Set("sort_code", bankPaymentType.Equals(BankPaymentType.FASTERPAYMENTS) ? paymentMethod.SortCode : null)
                          .Set("iban", bankPaymentType.Equals(BankPaymentType.SEPA) ? paymentMethod.Iban : null)
                          .Set("name", paymentMethod.AccountName);

                    var maskedValue = new Dictionary<string, string>();
                    maskedValue.Add("payment.destination.account_number", destination.GetValue<string>("account_number"));
                    maskedValue.Add("payment.destination.iban", destination.GetValue<string>("iban"));

                    MaskedValues = ProtectSensitiveData.HideValues(maskedValue, 4);

                    JsonDoc remittance_reference = new JsonDoc();
                    remittance_reference.Set("type", builder.RemittanceReferenceType != null ? builder.RemittanceReferenceType.ToString() : null)
                          .Set("value", builder.RemittanceReferenceValue);

                    payment.Set("scheme", bankPaymentType.ToString())
                           .Set("destination", destination);

                    if(remittance_reference.HasKeys())
                        payment.Set("remittance_reference", remittance_reference);

                    request.Set("order", order)
                           .Set("payment", payment)
                           .Set("return_url", paymentMethod.ReturnUrl)
                           .Set("status_url", paymentMethod.StatusUpdateUrl);

                    break;
                default:
                    break;
            }

            try
            {
                Request.MaskedValues = MaskedValues;

                string rawResponse = DoTransaction(HttpMethod.Post, "/payments", request.ToString());
                return OpenBankingMapping.MapResponse(rawResponse);
            }
            catch (GatewayException gatewayException)
            {
                throw gatewayException;
            }
        }

        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            string timestamp = GenerationUtils.GenerateTimestamp();

            switch (builder.ReportType) {
                case ReportType.FindBankPayment:
                    if (builder is TransactionReportBuilder<T>) {
                        var trb = builder as TransactionReportBuilder<T>;
                        var accountId = string.IsNullOrEmpty(trb.SearchBuilder.BankPaymentId) ? AccountId : "";
                        string hash = GenerationUtils.GenerateHash(SharedSecret, ShaHashType, timestamp, MerchantId, accountId,
                            !string.IsNullOrEmpty(trb.SearchBuilder.BankPaymentId) ? trb.SearchBuilder.BankPaymentId : "",
                            trb.SearchBuilder.StartDate.HasValue ? trb.SearchBuilder.StartDate.Value.ToString("yyyyMMddHHmmss") : "",
                            trb.SearchBuilder.EndDate.HasValue ? trb.SearchBuilder.EndDate.Value.ToString("yyyyMMddHHmmss") : "",
                            trb.SearchBuilder.ReturnPII.HasValue ? (trb.SearchBuilder.ReturnPII.Value ? "True" : "False") : "");

                        SetAuthorizationHeader(hash);

                        queryParams.Add("timestamp", timestamp);
                        queryParams.Add("merchantId", MerchantId);
                        if(!string.IsNullOrEmpty(accountId))
                        queryParams.Add("accountId", accountId);
                        var obTransId = !string.IsNullOrEmpty(trb.SearchBuilder.BankPaymentId) ? trb.SearchBuilder.BankPaymentId : "";
                        if(!string.IsNullOrEmpty(obTransId))
                        queryParams.Add("obTransId", obTransId);
                        var startDate = trb.SearchBuilder.StartDate.HasValue ? trb.SearchBuilder.StartDate.Value.ToString("yyyyMMddHHmmss") : "";
                        if(!string.IsNullOrEmpty(startDate))
                        queryParams.Add("startDateTime", startDate);
                        var endDate = trb.SearchBuilder.EndDate.HasValue ? trb.SearchBuilder.EndDate.Value.ToString("yyyyMMddHHmmss") : "";
                        if (!string.IsNullOrEmpty(endDate))
                            queryParams.Add("endDateTime", endDate);
                        var transState = trb.SearchBuilder.BankPaymentStatus.HasValue ? trb.SearchBuilder.BankPaymentStatus.Value.ToString() : "";
                        if (!string.IsNullOrEmpty(transState))
                            queryParams.Add("transactionState", transState);
                        var returnPii = trb.SearchBuilder.ReturnPII.HasValue ? (trb.SearchBuilder.ReturnPII.Value ? "True" : "False") : "";
                        if (!string.IsNullOrEmpty(returnPii))
                            queryParams.Add("returnPii", returnPii);

                    }
                    break;
                default:
                    break;
            }

            try
            {
                string response = DoTransaction(HttpMethod.Get, "/payments", null, queryParams);
                return OpenBankingMapping.MapReportResponse<T>(response, builder.ReportType);
            }
            catch (GatewayException ex)
            {
                throw ex;
            }
        }

        private void SetAuthorizationHeader(string value) {
            if (!Headers.ContainsKey("Authorization")) {
                Headers.Add("Authorization", $"{ShaHashType} {value}");
            }
            else {
                Headers["Authorization"] = $"{ShaHashType} {value}";                
            }
        }

        public static BankPaymentType? GetBankPaymentType(string currency) {
            switch (currency) {
                case "EUR":
                    return BankPaymentType.SEPA;
                case "GBP":
                    return BankPaymentType.FASTERPAYMENTS;
                default:
                    return null;
            }
        }

        protected override string HandleResponse(GatewayResponse response)
        {
            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
            {
                var parsed = JsonDoc.Parse(response.RawResponse);
                var error = parsed.Get("error") ?? parsed;
                throw new GatewayException(string.Format("Status Code: {0} - {1}", response.StatusCode, error.GetValue<string>("message")));
            }
            return response.RawResponse;
        }

        public Transaction ManageOpenBanking(ManagementBuilder builder)
        {
            string timestamp = builder.Timestamp ?? GenerationUtils.GenerateTimestamp();            
            var amount = builder.Amount != null ? builder.Amount.ToNumericCurrencyString() : null;

            JsonDoc request = new JsonDoc();                 
        
            switch (builder.TransactionType) {
            case TransactionType.Refund:
                   
               string hash = GenerationUtils.GenerateHash(SharedSecret, ShaHashType, MerchantId, AccountId, timestamp, builder.TransactionId, builder.ClientTransactionId, amount);
               
                SetAuthorizationHeader(hash);
                    request.Set("merchant_id", MerchantId)
                              .Set("account_id", AccountId)
                              .Set("request_timestamp", timestamp);

                    JsonDoc order = new JsonDoc();
                    order.Set("id", builder.TransactionId)
                          .Set("ob_trans_id", builder.ClientTransactionId)
                          .Set("amount", amount)
                          .Set("description", builder.Description);

                    request.Set("order", order);

                    break;
                default:
                    break;
            }

            try {
                string rawResponse = DoTransaction(HttpMethod.Post, "/refunds", request.ToString());
                return OpenBankingMapping.MapResponse(rawResponse);
            }
            catch (GatewayException gatewayException) {
                throw gatewayException;
            }
        }
    }
}
