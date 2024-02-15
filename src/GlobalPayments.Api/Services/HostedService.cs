using System.Collections.Generic;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Services {
    public class HostedService {
        GpEcomConfig _config;

        public HostedService(GpEcomConfig config, string configName = "default") {
            _config = config;
            ServicesContainer.ConfigureService(config, configName);
        }

        public AuthorizationBuilder Authorize(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Auth).WithAmount(amount);
        }

        public AuthorizationBuilder Charge(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Sale).WithAmount(amount);
        }

        public AuthorizationBuilder Verify(decimal? amount = null) {
            return new AuthorizationBuilder(TransactionType.Verify).WithAmount(amount);
        }

        public Transaction ParseResponse(string json, bool encoded = false) {
            var response = JsonDoc.Parse(json, encoded ? JsonEncoders.Base64Encoder : null);

            if (response.Has("MERCHANT_RESPONSE_URL")) {
                MapTransactionStatusResponse(ref response);
            }

            var timestamp = response.GetValue<string>("TIMESTAMP");
            var merchantId = response.GetValue<string>("MERCHANT_ID");
            var orderId = response.GetValue<string>("ORDER_ID");
            var result = response.GetValue<string>("RESULT");
            var message = response.GetValue<string>("MESSAGE");
            var transactionId = response.GetValue<string>("PASREF");
            var authCode = response.GetValue<string>("AUTHCODE") ?? null;
            var paymentMethod = response.GetValue<string>("PAYMENTMETHOD") ?? null;
            var sha1Hash = response.GetValue<string>($"{_config.ShaHashType}HASH");
            var hash = GenerationUtils.GenerateHash(_config.SharedSecret, _config.ShaHashType, timestamp, merchantId, orderId, result, message, transactionId, response.Has("MERCHANT_RESPONSE_URL") ? paymentMethod != null ? paymentMethod : authCode : authCode);
            if (!hash.Equals(sha1Hash))
                throw new ApiException("Incorrect hash. Please check your code and the Developers Documentation.");

            // remainder of the values
            var rvalues = new Dictionary<string, string>();
            foreach (var key in response.Keys) {
                var value = response.GetValue<string>(key);
                if (value != null)
                    rvalues.Add(key, value);
            }

            var transaction = new Transaction();           
            transaction.AuthorizedAmount = response.GetValue<decimal>("AMOUNT");
            transaction.AutoSettleFlag = response.GetValue<string>("AUTO_SETTLE_FLAG");
            transaction.CvnResponseCode = response.GetValue<string>("CVNRESULT");
            transaction.ResponseCode = result;
            transaction.ResponseMessage = message;
            transaction.AvsResponseCode = response.GetValue<string>("AVSPOSTCODERESULT");
            transaction.Timestamp = timestamp;
            transaction.TransactionReference = new TransactionReference {
                AuthCode = authCode,
                OrderId = orderId,
                PaymentMethodType = response.Has("PAYMENTMETHOD") ? PaymentMethodType.APM : PaymentMethodType.Credit,
                TransactionId = transactionId
            };
            transaction.ResponseValues = rvalues;
            
            if (response.Has("PAYMENTMETHOD")) {
                var apm = new AlternativePaymentResponse();
                apm.Country = response.GetValue<string>("COUNTRY") ?? "";
                apm.ProviderName = response.GetValue<string>("PAYMENTMETHOD");
                apm.PaymentStatus = response.GetValue<string>("TRANSACTION_STATUS") ?? null;
                apm.ReasonCode = response.GetValue<string>("PAYMENT_PURPOSE") ?? null;
                apm.AccountHolderName = response.GetValue<string>("ACCOUNT_HOLDER_NAME") ?? null;

                transaction.AlternativePaymentResponse = apm;
            }

            return transaction;
        }

        private void MapTransactionStatusResponse(ref JsonDoc response)
        {
            response.Set("ACCOUNT_HOLDER_NAME", response.GetValue<string>("accountholdername"))
            .Set("ACCOUNT_NUMBER", response.GetValue<string>("accountnumber"))
            .Set("TIMESTAMP", response.GetValue<string>("timestamp"))
            .Set("MERCHANT_ID", response.GetValue<string>("merchantid"))
            .Set("BANK_CODE", response.GetValue<string>("bankcode"))
            .Set("BANK_NAME", response.GetValue<string>("bankname"))
            .Set("HPP_CUSTOMER_BIC", response.GetValue<string>("bic"))
            .Set("COUNTRY", response.GetValue<string>("country"))
            .Set("HPP_CUSTOMER_EMAIL", response.GetValue<string>("customeremail"))
            .Set("TRANSACTION_STATUS", response.GetValue<string>("fundsstatus"))
            .Set("IBAN", response.GetValue<string>("iban"))
            .Set("MESSAGE", response.GetValue<string>("message"))
            .Set("ORDER_ID", response.GetValue<string>("orderid"))
            .Set("PASREF", response.GetValue<string>("pasref"))
            .Set("PAYMENTMETHOD", response.GetValue<string>("paymentmethod"))
            .Set("PAYMENT_PURPOSE", response.GetValue<string>("paymentpurpose"))
            .Set("RESULT", response.GetValue<string>("result"))
            .Set($"{_config.ShaHashType}HASH", response.GetValue<string>($"{_config.ShaHashType.ToString().ToLower()}hash"));
            
        }
    }
}
