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

            var timestamp = response.GetValue<string>("TIMESTAMP");
            var merchantId = response.GetValue<string>("MERCHANT_ID");
            var orderId = response.GetValue<string>("ORDER_ID");
            var result = response.GetValue<string>("RESULT");
            var message = response.GetValue<string>("MESSAGE");
            var transactionId = response.GetValue<string>("PASREF");
            var authCode = response.GetValue<string>("AUTHCODE");
            var sha1Hash = response.GetValue<string>("SHA1HASH");
            var hash = GenerationUtils.GenerateHash(_config.SharedSecret, timestamp, merchantId, orderId, result, message, transactionId, authCode);
            if (!hash.Equals(sha1Hash))
                throw new ApiException("Incorrect hash. Please check your code and the Developers Documentation.");

            // remainder of the values
            var rvalues = new Dictionary<string, string>();
            foreach (var key in response.Keys) {
                var value = response.GetValue<string>(key);
                if (value != null)
                    rvalues.Add(key, value);
            }

            return new Transaction {
                AuthorizedAmount = response.GetValue<decimal>("AMOUNT"),
                AutoSettleFlag = response.GetValue<string>("AUTO_SETTLE_FLAG"),
                CvnResponseCode = response.GetValue<string>("CVNRESULT"),
                ResponseCode = result,
                ResponseMessage = message,
                AvsResponseCode = response.GetValue<string>("AVSPOSTCODERESULT"),
                Timestamp = timestamp,
                TransactionReference = new TransactionReference {
                    AuthCode = authCode,
                    OrderId = orderId,
                    PaymentMethodType = PaymentMethodType.Credit,
                    TransactionId = transactionId
                },
                ResponseValues = rvalues
            };
        }
    }
}
