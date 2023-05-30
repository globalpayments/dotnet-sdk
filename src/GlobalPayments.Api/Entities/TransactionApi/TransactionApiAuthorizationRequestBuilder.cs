using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class TransactionApiAuthorizationRequestBuilder {
        internal static TransactionApiRequest BuildRequest(AuthorizationBuilder builder, TransactionApiConnector gateway) {
            var serviceUrl = gateway.ServiceUrl;
            SetDynamicHeader(builder, gateway);
            var doc = new JsonDoc();
            var card = new JsonDoc();
            if (builder.PaymentMethod is ICardData) {
                var tokenKeyName = "token";
                if (builder.PaymentMethodUsageMode == PaymentMethodUsageMode.Single)
                    tokenKeyName = "temporary_token";
                var cardData = builder.PaymentMethod as ICardData;
                card.Set("card_number", ((CreditCardData)builder.PaymentMethod).Token == null ? cardData.Number: null)
                .Set("card_security_code", cardData.Cvn)
                .Set("cardholder_name", ((CreditCardData)builder.PaymentMethod).CardHolderName)
                .Set(tokenKeyName, ((CreditCardData)builder.PaymentMethod).Token != null ? ((CreditCardData)builder.PaymentMethod).Token : null);
                if (!string.IsNullOrEmpty(cardData.Number) && ((CreditCardData)builder.PaymentMethod).Token == null) {
                    card.Set("expiry_month", cardData.ExpMonth.HasValue ? cardData.ExpMonth.ToString().PadLeft(2, '0') : null)
                    .Set("expiry_year", cardData.ExpYear.HasValue ? cardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : null);
                }
                doc.Set("card", card);
            }

            // Added check Details.
            if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH && builder.PaymentMethod is eCheck) {
                var checkDetails = builder.PaymentMethod as eCheck;
                card.Set("account_type", EnumConverter.GetMapping(Target.TransactionApi, checkDetails.AccountType));
                    if(string.IsNullOrEmpty(checkDetails.Token)) {
                        card.Set("account_number", checkDetails.AccountNumber)
                            .Set("routing_number", checkDetails.RoutingNumber)
                            .Set("check_number", checkDetails.CheckNumber)
                            .Set("branch_transit_number", checkDetails.BranchTransitNumber)
                            .Set("financial_institution_number", checkDetails.FinancialInstitutionNumber);
                    } 
                    else {
                        card.Set("token", checkDetails.Token);
                    }
                doc.Set("check", card);
            }
            
            if (builder.Customer != null) {
                var customer = new JsonDoc()
                .Set("id", builder.Customer?.Id);
                if (string.IsNullOrEmpty(builder.Customer?.BusinessName)) {
                    customer.Set("title", builder.Customer?.Title)
                        .Set("first_name", builder.Customer?.FirstName)
                        .Set("middle_name", builder.Customer?.MiddleName)
                        .Set("last_name", builder.Customer?.LastName);
                }
                else {
                    customer.Set("business_name", builder.Customer?.BusinessName);
                }
                
                customer.Set("email", builder.Customer?.Email)
                .Set("phone", builder.Customer?.MobilePhone)
                .Set("note", builder.Customer?.Comments);
                if(builder.BillingAddress != null && builder.PaymentMethod.PaymentMethodType != PaymentMethodType.ACH) {
                    var billingAddress = new JsonDoc()
                    .Set("line1", builder.BillingAddress?.StreetAddress1)
                    .Set("line2", builder.BillingAddress?.StreetAddress2)
                    .Set("city", builder.BillingAddress?.City)
                    .Set("state", builder.BillingAddress?.State)
                    .Set("country", builder.BillingAddress?.Country)
                    .Set("postal_code", builder.BillingAddress?.PostalCode);
                    customer.Set("billing_address", billingAddress);
                }
                
                doc.Set("customer", customer);
            }

            var paymentData = new JsonDoc()
                .Set("amount", builder.Amount.ToString())
                .Set("currency_code", builder.Currency)
                .Set("gratuity_amount", builder?.Gratuity)
                .Set("invoice_number", builder.InvoiceNumber);
            doc.Set("payment", paymentData);

            if(builder.ShippingAddress != null) {
                    var shipping = new JsonDoc()
                    .Set("date", builder.ShippingDate.ToString("yyyy-MM-dd"));
                    var ShippingAddress = new JsonDoc()
                        .Set("line1", builder.BillingAddress?.StreetAddress1)
                        .Set("line2", builder.BillingAddress?.StreetAddress2)
                        .Set("city", builder.BillingAddress?.City)
                        .Set("state", builder.BillingAddress?.State)
                        .Set("country", builder.BillingAddress?.Country)
                        .Set("postal_code", builder.BillingAddress?.PostalCode);
                    shipping.Set("address", ShippingAddress);
                    doc.Set("shipping", shipping);
                }

            var transactionData = new JsonDoc();
            if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Credit &&  builder.TransactionType == TransactionType.Refund) {
                transactionData.Set("country_code", EnumConverter.GetDescription(builder.TransactionData?.CountryCode))
                .Set("language", EnumConverter.GetDescription(builder.TransactionData?.Language))
                .Set("soft_descriptor", builder.TransactionData?.SoftDescriptor);
            }
            else {
                transactionData.Set("country_code", EnumConverter.GetDescription(builder.TransactionData?.CountryCode))
                .Set("ecommerce_indicator", EnumConverter.GetDescription(builder.TransactionData?.EcommerceIndicator))
                .Set("language", EnumConverter.GetDescription(builder.TransactionData?.Language))
                .Set("soft_descriptor", builder.TransactionData?.SoftDescriptor)
                .Set("payment_purpose_code", builder.TransactionData?.PaymentPurposeCode)
                .Set("entry_class", builder.TransactionData?.EntryClass);
            }
            var indData = new JsonDoc();
            indData.Set("create_token", builder.RequestMultiUseToken);
            if(builder.PaymentMethod.PaymentMethodType == PaymentMethodType.ACH && builder.TransactionType == TransactionType.Sale) {
                indData.Set("check_verify", false);
            }
            else {
                indData.Set("address_verification_service", builder.TransactionData?.AddressVerificationService)
                .Set("generate_receipt", builder.TransactionData?.GenerateReceipt)
                .Set("partial_approval", builder.TransactionData?.PartialApproval);
            }
            if(builder.TransactionType != TransactionType.Refund)
                transactionData.Set("processing_indicators", indData);
            doc.Set("transaction", transactionData);

            if (!string.IsNullOrEmpty(builder.ClerkId)) {
                var receipt = new JsonDoc()
                .Set("clerk_id", builder.ClerkId);
                doc.Set("receipt", receipt);
            }

            if (string.IsNullOrEmpty(builder.ClientTransactionId)) {
                throw new BuilderException("Reference Id is required field.");
            }
            doc.Set("reference_id", builder.ClientTransactionId);

            var docRequest = doc.ToString();
            
            return new TransactionApiRequest {
                Verb = HttpMethod.Post,
                Endpoint = $"/" + GetEndPoint(builder.TransactionType, builder.PaymentMethod.PaymentMethodType),
                RequestBody = doc.ToString(),
            };
        }

        private static void SetDynamicHeader(AuthorizationBuilder builder, TransactionApiConnector gateway) {
            if(builder.TransactionType == TransactionType.Auth || builder.TransactionType == TransactionType.Sale || builder.TransactionType == TransactionType.Refund || builder.TransactionType == TransactionType.Verify) {
                gateway.Headers["X-GP-Partner-App-Name"] = "mobile_sdk";
                gateway.Headers["X-GP-Partner-App-Version"] = "1";
            }
        }

        private static string GetEndPoint(TransactionType type, PaymentMethodType methodType) {
            var endPoint = "";
            switch (type) {
                case TransactionType.Auth:
                case TransactionType.Verify:
                    endPoint = "creditauths";
                    break;
                case TransactionType.Refund:
                        endPoint = methodType == PaymentMethodType.ACH ? "checkrefunds" : "creditreturns";
                    break;
                case TransactionType.Sale:
                    endPoint = methodType == PaymentMethodType.ACH ? "checksales" : "creditsales";
                    break;
                default:
                    break;
            }
            return endPoint;
        }
    }
}
