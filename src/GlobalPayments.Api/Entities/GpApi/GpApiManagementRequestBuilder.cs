using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class GpApiManagementRequestBuilder {
        internal static GpApiRequest BuildRequest(ManagementBuilder builder, GpApiConnector gateway) {
            if (builder.TransactionType == TransactionType.Capture) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("gratuity_amount", builder.Gratuity.ToNumericCurrencyString());

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"/transactions/{builder.TransactionId}/capture",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.Refund) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString());

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"/transactions/{builder.TransactionId}/refund",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.Reversal) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString());

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"/transactions/{builder.TransactionId}/reversal",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.TokenUpdate && builder.PaymentMethod is CreditCardData) {
                var cardData = builder.PaymentMethod as CreditCardData;

                var card = new JsonDoc()
                    .Set("expiry_month", cardData.ExpMonth.HasValue ? cardData.ExpMonth.ToString().PadLeft(2, '0') : string.Empty)
                    .Set("expiry_year", cardData.ExpYear.HasValue ? cardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty);

                var data = new JsonDoc()
                    .Set("card", card);

                return new GpApiRequest {
                    Verb = new HttpMethod("PATCH"),
                    Endpoint = $"/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.TokenDelete && builder.PaymentMethod is ITokenizable) {
                return new GpApiRequest {
                    Verb = HttpMethod.Delete,
                    Endpoint = $"/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}",
                };
            }
            else if (builder.TransactionType == TransactionType.Detokenize && builder.PaymentMethod is ITokenizable) {
                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}/detokenize",
                };
            }
            else if (builder.TransactionType == TransactionType.DisputeAcceptance) {
                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"/disputes/{builder.DisputeId}/acceptance",
                };
            }
            else if (builder.TransactionType == TransactionType.DisputeChallenge) {
                var data = new JsonDoc()
                    .Set("documents", builder.DisputeDocuments);

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"/disputes/{builder.DisputeId}/challenge",
                    RequestBody = data.ToString(),
                };
            }
            return null;
        }
    }
}
