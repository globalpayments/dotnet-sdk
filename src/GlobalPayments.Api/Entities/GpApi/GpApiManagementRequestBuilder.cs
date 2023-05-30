﻿using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Net.Http;

namespace GlobalPayments.Api.Entities
{
    internal class GpApiManagementRequestBuilder {

        private static Dictionary<string, List<string>> AllowedActions { get; set; }
               
        internal static GpApiRequest BuildRequest(ManagementBuilder builder, GpApiConnector gateway) {
            GetAllowedActions();

            var merchantUrl = !string.IsNullOrEmpty(gateway.GpApiConfig.MerchantId) ? $"/merchants/{gateway.GpApiConfig.MerchantId}" : string.Empty;
          
            if (builder.PaymentMethod?.PaymentMethodType == PaymentMethodType.BankPayment) {
                var message = $"The {builder.TransactionType.ToString()} is not supported for {PaymentMethodName.BankPayment.ToString()}";
                
                if (AllowedActions[PaymentMethodType.BankPayment.ToString()] == null) {
                    throw new BuilderException(message);                    
                }
                else if (!AllowedActions[PaymentMethodType.BankPayment.ToString()].Contains(builder.TransactionType.ToString())) {
                    throw new BuilderException(message);
                }
            }
            
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

                var endpoint = $"{merchantUrl}";

                if (builder.PaymentMethod.PaymentMethodType == PaymentMethodType.Account_Funds) {
                    if (!string.IsNullOrEmpty(builder.FundsData?.MerchantId)) {
                        endpoint = $"/merchants/{builder.FundsData.MerchantId}";
                    }
                    endpoint += $"/transfers/{builder.TransactionId}/reversal";
                }
                else {
                    endpoint += $"/transactions/{builder.TransactionId}/reversal";
                }

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = endpoint,
                    RequestBody = data.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.TokenUpdate) {
                if(!(builder.PaymentMethod is CreditCardData)) {
                    throw new GatewayException("Payment method doesn't support this action!");
                }

                var cardData = builder.PaymentMethod as CreditCardData;

                var card = new JsonDoc()
                    .Set("expiry_month", cardData.ExpMonth.HasValue ? cardData.ExpMonth.ToString().PadLeft(2, '0') : string.Empty)
                    .Set("expiry_year", cardData.ExpYear.HasValue ? cardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : string.Empty);

                var data = new JsonDoc()
                    .Set("usage_mode", !string.IsNullOrEmpty(builder.PaymentMethodUsageMode.ToString()) ? EnumConverter.GetMapping(Target.GP_API, builder.PaymentMethodUsageMode) : null)
                    .Set("name", !string.IsNullOrEmpty(cardData.CardHolderName) ? cardData.CardHolderName : null)
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
            else if (builder.TransactionType == TransactionType.Auth)
            {
                var payload = new JsonDoc();
                payload.Set("amount", builder.Amount);

                if (builder.LodgingData != null) {
                    var lodging = builder.LodgingData;
                    if (lodging.Items != null) {
                        var lodginItems = new List<LodgingItems>();

                        foreach (var item in lodging.Items)
                        {
                            lodginItems.Add(new LodgingItems { 
                                Types = item.Types,
                                Reference = item.Reference,
                                TotalAmount = item.TotalAmount,
                                paymentMethodProgramCodes = item.paymentMethodProgramCodes
                            });
                        }

                        var lodgingData = new JsonDoc()
                      .Set("booking_reference", lodging.bookingReference)
                      .Set("duration_days", lodging.StayDuration)
                      .Set("date_checked_in", lodging.CheckInDate?.ToString("yyyy-MM-dd"))
                      .Set("date_checked_out", lodging.CheckOutDate?.ToString("yyyy-MM-dd"))
                      .Set("daily_rate_amount", lodging.Rate.ToNumericCurrencyString())
                      .Set("charge_items", lodginItems);

                      payload.Set("lodging", lodgingData);
                    }                    
                }
                return new GpApiRequest
                {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/transactions/{builder.TransactionId}/incremental",
                    RequestBody = payload.ToString(),
                };
            }
            else if (builder.TransactionType == TransactionType.Edit)
            {                
                    var card = new JsonDoc()
                        .Set("tag", builder.TagData);

                    var payment_method = new JsonDoc()
                        .Set("card", card);

                    var payload = new JsonDoc()
                        .Set("amount", builder.Amount.ToNumericCurrencyString())
                        .Set("gratuity_amount", builder.Gratuity.ToNumericCurrencyString())
                        .Set("payment_method", payment_method);

                    return new GpApiRequest
                    {
                        Verb = HttpMethod.Post,
                        Endpoint = $"{merchantUrl}/transactions/{builder.TransactionId}/adjustment",
                        RequestBody = payload.ToString(),
                    };
                
            }
            else if (builder.TransactionType == TransactionType.PayLinkUpdate)
            {
                var payLinkData = builder.PayLinkData;

                var payload = new JsonDoc()
                    .Set("usage_mode", EnumConverter.GetMapping(Target.GP_API, payLinkData.UsageMode) ?? null)
                    .Set("usage_limit", payLinkData.UsageLimit ?? null)
                    .Set("name", payLinkData.Name ?? null)
                    .Set("description", builder.Description ?? null)
                    .Set("type", payLinkData.Type.ToString() ?? null)
                    .Set("status", payLinkData.Status.ToString() ?? null)
                    .Set("shippable", payLinkData.IsShippable != null && payLinkData.IsShippable.Value ? "YES" : "NO" )
                    .Set("shipping_amount", payLinkData.ShippingAmount.ToNumericCurrencyString());

                var transaction = new JsonDoc()
                    .Set("amount", builder.Amount.ToNumericCurrencyString() ?? null);

                payload.Set("transactions", transaction)
                    .Set("expiration_date", payLinkData.ExpirationDate ?? null)
                    .Set("images", payLinkData.Images ?? null);

                return new GpApiRequest
                {
                    Verb = new HttpMethod("PATCH"),
                    Endpoint = $"{merchantUrl}/links/{builder.PaymentLinkId}",
                    RequestBody = payload.ToString(),
                };

            }
            else if (builder.TransactionType == TransactionType.Release || builder.TransactionType == TransactionType.Hold)
            {
                var payload = new JsonDoc()
                    .Set("reason_code", EnumConverter.GetMapping(Target.GP_API, builder.ReasonCode) ?? null);

                var endpoint = builder.TransactionType == TransactionType.Release ? "release" : builder.TransactionType == TransactionType.Hold ? "hold" : null;

                return new GpApiRequest
                {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{merchantUrl}/transactions/{builder.TransactionId}/{endpoint}",
                    RequestBody = payload.ToString(),
                };
            }

            //Transaction split
            else if(builder.TransactionType == TransactionType.SplitFunds) {
                var payment = builder.PaymentMethod;                
                var transfer = new JsonDoc();
                var split = new List<Dictionary<string, object>>();

                var request = new Dictionary<string, object>();
                request.Add("recipient_account_id", builder.FundsData.RecipientAccountId);
                request.Add("reference", builder.Reference);
                request.Add("description", builder.Description);
                request.Add("amount", builder.Amount.ToNumericCurrencyString());

                split.Add(request);

                transfer.Set("transfers", split);
                                
                var endpoint = merchantUrl;
                if (!string.IsNullOrEmpty(builder.FundsData.MerchantId)) {
                    endpoint = $"/merchants/{builder.FundsData.MerchantId}";
                }

                return new GpApiRequest {
                    Verb = HttpMethod.Post,
                    Endpoint = $"{endpoint}/transactions/{builder.TransactionId}/split",
                    RequestBody = transfer.ToString(),
                };
            }
            
            return null;
        }

        private static void GetAllowedActions()
        {
            if (AllowedActions == null) {
                AllowedActions = new Dictionary<string, List<string>>();
                AllowedActions.Add(PaymentMethodType.BankPayment.ToString(), null);
            }
        }
    }
}
