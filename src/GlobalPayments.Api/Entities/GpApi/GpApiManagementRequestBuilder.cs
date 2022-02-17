using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class GpApiManagementRequestBuilder {
        internal static GpApiRequest BuildRequest(ManagementBuilder builder, GpApiConnector gateway) {
            var merchantUrl = !string.IsNullOrEmpty(gateway.MerchantId) ? $"/merchants/{gateway.MerchantId}" : string.Empty;
            if (builder.TransactionType == TransactionType.Capture) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("gratuity_amount", builder.Gratuity.ToNumericCurrencyString())
                    .Set("currency_conversion", builder.DccRateData?.DccId ?? null);

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/transactions/{builder.TransactionId}/capture",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.Refund) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("currency_conversion", builder.DccRateData == null ? null : new JsonDoc()
                        .Set("id", builder.DccRateData?.DccId));

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/transactions/{builder.TransactionId}/refund",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.Reversal) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString())
                    .Set("currency_conversion", builder.DccRateData?.DccId ?? null);

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/transactions/{builder.TransactionId}/reversal",
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
                    Endpoint = $"{merchantUrl}/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.TokenDelete && builder.PaymentMethod is ITokenizable) {
                return new GpApiRequest {
                    Verb = HttpMethod.Delete,
                    Endpoint = $"{merchantUrl}/payment-methods/{(builder.PaymentMethod as ITokenizable).Token}",
                };
            }
            else if (builder.TransactionType == TransactionType.DisputeAcceptance) {
                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/disputes/{builder.DisputeId}/acceptance",
                };
            }
            else if (builder.TransactionType == TransactionType.DisputeChallenge) {
                var data = new JsonDoc()
                    .Set("documents", builder.DisputeDocuments);

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/disputes/{builder.DisputeId}/challenge",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.BatchClose) {
                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/batches/{builder.BatchReference}",
                };
            }
            else if (builder.TransactionType == TransactionType.Reauth) {
                var data = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString());
                if(builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH)
                {
                    data.Set("description", builder.Description);
                    if(builder.BankTransferDetails != null)
                    {
                        var eCheck = builder.BankTransferDetails;

                        var paymentMethod = new JsonDoc()
                            .Set("narrative", eCheck.MerchantNotes);

                        var bankTransfer = new JsonDoc()
                            .Set("account_number", eCheck.AccountNumber)
                            .Set("account_type", (eCheck.AccountType != null) ? EnumConverter.GetMapping(Target.GP_API, eCheck.AccountType) : null)
                            .Set("check_reference", eCheck.CheckReference);

                        var bank = new JsonDoc()
                            .Set("code", eCheck.RoutingNumber)
                            .Set("name", eCheck.BankName);

                        bankTransfer.Set("bank", bank);

                        paymentMethod.Set("bank_transfer", bankTransfer);

                        data.Set("payment_method", paymentMethod);
                    }
                }

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/transactions/{builder.TransactionId}/reauthorization",
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.Confirm)
            {
                if (builder.PaymentMethod is TransactionReference && builder.PaymentMethod.PaymentMethodType == PaymentMethodType.APM)
                {
                    var transactionReference = (TransactionReference) builder.PaymentMethod;
                    var apmResponse = transactionReference.AlternativePaymentResponse;
                    var apm = new JsonDoc()
                        .Set("provider", apmResponse?.ProviderName)
                        .Set("provider_payer_reference", apmResponse?.ProviderReference);

                    var payment_method = new JsonDoc()
                        .Set("apm", apm);

                    var data = new JsonDoc()
                        .Set("payment_method", payment_method);

                    return new GpApiRequest
                    {
                        Verb = HttpMethod.Post,
                        Endpoint = $"{merchantUrl}/transactions/{builder.TransactionId}/confirmation",
                        RequestBody = data.ToString(),
                    };
                }
            }
            return null;
        }
    }
}
