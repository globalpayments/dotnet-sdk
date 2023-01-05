using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class TransactionApiManagementRequestBuilder {
        internal static TransactionApiRequest BuildRequest(ManagementBuilder builder, TransactionApiConnector gateway) {
            var serviceUrl = gateway.ServiceUrl;
            SetDynamicHeader(builder, gateway);
            var doc = new JsonDoc();
            var reference = (TransactionReference)builder.PaymentMethod;
            if (builder.TransactionType != TransactionType.Fetch) {
                var card = new JsonDoc();
                if (builder.TransactionType != TransactionType.Void && builder.TransactionType != TransactionType.Edit && (builder.TransactionType == TransactionType.Refund && builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH)) {
                    // Added check Details.
                    card.Set("account_type", EnumConverter.GetMapping(Target.TransactionApi, builder.BankTransferDetails.AccountType))
                    .Set("check_number", builder.BankTransferDetails.CheckNumber);
                    doc.Set("check", card);
                }
                var paymentData = new JsonDoc()
                .Set("amount", builder.Amount.ToString())
                .Set("invoice_number", builder.InvoiceNumber);
                if (builder.Gratuity.HasValue)
                    paymentData.Set("gratuity_amount", builder.Gratuity.ToString());
                doc.Set("payment", paymentData);

                var transactionData = new JsonDoc()
                    .Set("entry_class", builder.EntryClass)
                    .Set("payment_purpose_code", builder.PaymentPurposeCode)
                    .Set("soft_descriptor", builder.SoftDescriptor);
                if (builder.TransactionType == TransactionType.Edit || (builder.TransactionType == TransactionType.Refund && builder.PaymentMethod.PaymentMethodType != PaymentMethodType.ACH)) {
                    var indData = new JsonDoc()
                        .Set("allow_duplicate", builder.AllowDuplicates)
                        .Set("generate_receipt", builder.GenerateReceipt);
                    transactionData.Set("processing_indicators", indData);
                }
                if (builder.TransactionType == TransactionType.Void) {
                    var processingIndicator = new JsonDoc()
                    .Set("generate_receipt", builder.GenerateReceipt);
                    transactionData.Set("processing_indicators", processingIndicator);
                }

                doc.Set("transaction", transactionData);

                if (!string.IsNullOrEmpty(builder.ClerkId)) {
                    var receipt = new JsonDoc()
                    .Set("clerk_id", builder.ClerkId);
                    doc.Set("receipt", receipt);
                }
            }
            var docRequest = doc.ToString();
            var endPoint = BuildEndPoint(builder);
            
            return new TransactionApiRequest {
                Verb = GetVerb(builder.TransactionType),
                Endpoint = $"/" + endPoint,
                RequestBody = doc.ToString(),
            };
        }

        private static string BuildEndPoint(ManagementBuilder builder) {
            var endPoint = "";
            var reference = (TransactionReference)builder.PaymentMethod;
            if(reference != null) {
                switch (builder.TransactionType) {
                    case TransactionType.Refund:
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH)
                            endPoint = reference.TransactionId != null ? $"/checksales/{reference.TransactionId}/checkrefunds" : $"/checksales/reference_id/{reference.ClientTransactionId}/checkrefunds";
                        if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit)
                            endPoint = reference.TransactionId != null ? $"/creditsales/{reference.TransactionId}/creditreturns" : $"/creditsales/reference_id/{reference.ClientTransactionId}/creditreturns";
                        break;
                    case TransactionType.Edit:
                        endPoint = reference.TransactionId != null ? $"/creditsales/{reference.TransactionId}" : $"/creditsales/reference_id/{reference.ClientTransactionId}";
                        break;
                    case TransactionType.Void:
                        if (reference.TransactionId != null) {
                            endPoint = reference.OriginalTransactionType == TransactionType.Sale ? $"/creditsales/{reference.TransactionId}/voids" : $"/creditreturns/{reference.TransactionId}/voids";
                        }
                        else {
                            endPoint = reference.OriginalTransactionType == TransactionType.Sale ? $"/creditsales/reference_id/{reference.ClientTransactionId}/voids" : $"/creditreturns/reference_id/{reference.ClientTransactionId}/voids";
                        }
                        break;
                    case TransactionType.Fetch:
                        if (reference.TransactionId != null) {
                            if(builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit)
                                endPoint = reference.OriginalTransactionType == TransactionType.Sale ? $"/creditsales/{reference.TransactionId}" : $"creditreturns/{reference.TransactionId}";
                            else
                                endPoint = reference.OriginalTransactionType == TransactionType.Sale ? $"checksales/{reference.TransactionId}" : $"/checkrefunds/{reference.TransactionId}";
                        }
                        else {
                            if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit)
                                endPoint = reference.OriginalTransactionType == TransactionType.Sale ? $"/creditsales/reference_id/{reference.ClientTransactionId}" : $"creditreturns/reference_id/{reference.ClientTransactionId}";
                            else
                            endPoint = reference.OriginalTransactionType == TransactionType.Sale ? $"/checksales/reference_id/{reference.ClientTransactionId}" : $"/checkrefunds/reference_id/{reference.ClientTransactionId}";
                        }
                break;
                    default:
                        break;
                }
            }
            return endPoint;
        }

        private static HttpMethod GetVerb(TransactionType transType) {
            if (transType == TransactionType.Void)
               return HttpMethod.Put;
            if (transType == TransactionType.Edit)
                return new HttpMethod("Patch");
            if (transType == TransactionType.Fetch)
                return HttpMethod.Get;
            return HttpMethod.Post;
        }

        private static void SetDynamicHeader(ManagementBuilder builder, TransactionApiConnector gateway) {
            if (builder.TransactionType == TransactionType.Auth || builder.TransactionType == TransactionType.Sale || builder.TransactionType == TransactionType.Refund ||
                builder.TransactionType == TransactionType.Void || builder.TransactionType == TransactionType.Edit || builder.TransactionType == TransactionType.Fetch) {
                gateway.Headers["X-GP-Partner-App-Name"] = "mobile_sdk";
                gateway.Headers["X-GP-Partner-App-Version"] = "1";
            }
        }
    }
}
