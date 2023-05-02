using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System.Net.Http;

namespace GlobalPayments.Api.Entities {
    internal class GpApiReportRequestBuilder {
        internal static GpApiRequest BuildRequest<T>(ReportBuilder<T> builder, GpApiConnector gateway) where T : class {
            var merchantUrl = !string.IsNullOrEmpty(gateway.GpApiConfig.MerchantId) ? $"/merchants/{gateway.GpApiConfig.MerchantId}" : string.Empty;
            var request = new GpApiRequest();
            if (builder is TransactionReportBuilder<T> trb) {                
                switch (builder.ReportType) {
                    case ReportType.TransactionDetail:
                        return new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/transactions/{trb.TransactionId}",
                        };
                    case ReportType.FindTransactionsPaged:
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/transactions",
                        };
                        request.AddQueryStringParam("page", trb.Page?.ToString());
                        request.AddQueryStringParam("page_size", trb.PageSize?.ToString());
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, trb.TransactionOrderBy));
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, trb.Order));
                        request.AddQueryStringParam("id", trb.TransactionId);
                        request.AddQueryStringParam("type", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.PaymentType));
                        request.AddQueryStringParam("channel", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.Channel));
                        request.AddQueryStringParam("amount", trb.SearchBuilder.Amount.ToNumericCurrencyString());
                        request.AddQueryStringParam("currency", trb.SearchBuilder.Currency);
                        request.AddQueryStringParam("number_first6", trb.SearchBuilder.CardNumberFirstSix);
                        request.AddQueryStringParam("number_last4", trb.SearchBuilder.CardNumberLastFour);
                        request.AddQueryStringParam("token_first6", trb.SearchBuilder.TokenFirstSix);
                        request.AddQueryStringParam("token_last4", trb.SearchBuilder.TokenLastFour);
                        request.AddQueryStringParam("account_name", trb.SearchBuilder.AccountName);
                        request.AddQueryStringParam("brand", trb.SearchBuilder.CardBrand);
                        request.AddQueryStringParam("brand_reference", trb.SearchBuilder.BrandReference);
                        request.AddQueryStringParam("authcode", trb.SearchBuilder.AuthCode);
                        request.AddQueryStringParam("reference", trb.SearchBuilder.ReferenceNumber);
                        request.AddQueryStringParam("status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.TransactionStatus));
                        request.AddQueryStringParam("from_time_created", trb.StartDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_time_created", trb.EndDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("country", trb.SearchBuilder.Country);
                        request.AddQueryStringParam("batch_id", trb.SearchBuilder.BatchId);
                        request.AddQueryStringParam("entry_mode", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.PaymentEntryMode));
                        request.AddQueryStringParam("name", trb.SearchBuilder.Name);
                        request.AddQueryStringParam("payment_method", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.PaymentMethodName));
                        if (trb.SearchBuilder.RiskAssessmentMode != null) {
                            request.AddQueryStringParam("risk_assessment_mode", trb.SearchBuilder.RiskAssessmentMode.ToString());
                        }
                        if (trb.SearchBuilder.RiskAssessmentResult != null) {
                            request.AddQueryStringParam("risk_assessment_result", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.RiskAssessmentResult));
                        }
                        if (trb.SearchBuilder.RiskAssessmentReasonCode != null) {
                            request.AddQueryStringParam("risk_assessment_reason_code", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.RiskAssessmentReasonCode));
                        }
                        if (trb.SearchBuilder.PaymentProvider != null) {
                            request.AddQueryStringParam("provider", trb.SearchBuilder.PaymentProvider.ToString());
                        }

                        return request;
                    case ReportType.FindSettlementTransactionsPaged:
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/settlement/transactions",
                        };
                        request.AddQueryStringParam("page", trb.Page?.ToString());
                        request.AddQueryStringParam("page_size", trb.PageSize?.ToString());
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, trb.TransactionOrderBy));
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, trb.Order));
                        request.AddQueryStringParam("number_first6", trb.SearchBuilder.CardNumberFirstSix);
                        request.AddQueryStringParam("number_last4", trb.SearchBuilder.CardNumberLastFour);
                        request.AddQueryStringParam("deposit_status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DepositStatus));
                        request.AddQueryStringParam("account_name", gateway.GpApiConfig.AccessTokenInfo.DataAccountName);
                        request.AddQueryStringParam("account_id", gateway.GpApiConfig.AccessTokenInfo.DataAccountID);
                        request.AddQueryStringParam("brand", trb.SearchBuilder.CardBrand);
                        request.AddQueryStringParam("arn", trb.SearchBuilder.AquirerReferenceNumber);
                        request.AddQueryStringParam("brand_reference", trb.SearchBuilder.BrandReference);
                        request.AddQueryStringParam("authcode", trb.SearchBuilder.AuthCode);
                        request.AddQueryStringParam("reference", trb.SearchBuilder.ReferenceNumber);
                        request.AddQueryStringParam("status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.TransactionStatus));
                        request.AddQueryStringParam("from_time_created", trb.StartDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_time_created", trb.EndDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("deposit_id", trb.SearchBuilder.DepositReference);
                        request.AddQueryStringParam("from_deposit_time_created", trb.SearchBuilder.StartDepositDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_deposit_time_created", trb.SearchBuilder.EndDepositDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("from_batch_time_created", trb.SearchBuilder.StartBatchDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_batch_time_created", trb.SearchBuilder.EndBatchDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("system.mid", trb.SearchBuilder.MerchantId);
                        request.AddQueryStringParam("system.hierarchy", trb.SearchBuilder.SystemHierarchy);

                        return request;
                    case ReportType.DepositDetail:
                        return new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/settlement/deposits/{trb.SearchBuilder.DepositReference}",
                        };
                    case ReportType.FindDepositsPaged:
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/settlement/deposits",
                        };
                        request.AddQueryStringParam("page", trb.Page?.ToString());
                        request.AddQueryStringParam("page_size", trb.PageSize?.ToString());
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, trb.DepositOrderBy));
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, trb.Order));
                        request.AddQueryStringParam("account_name", gateway.GpApiConfig.AccessTokenInfo.DataAccountName);
                        request.AddQueryStringParam("account_id", gateway.GpApiConfig.AccessTokenInfo.DataAccountID);
                        request.AddQueryStringParam("from_time_created", trb.StartDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_time_created", trb.EndDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("id", trb.SearchBuilder.DepositReference);
                        request.AddQueryStringParam("status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DepositStatus));
                        request.AddQueryStringParam("amount", trb.SearchBuilder.Amount.ToNumericCurrencyString());
                        request.AddQueryStringParam("masked_account_number_last4", trb.SearchBuilder.AccountNumberLastFour);
                        request.AddQueryStringParam("system.mid", trb.SearchBuilder.MerchantId);
                        request.AddQueryStringParam("system.hierarchy", trb.SearchBuilder.SystemHierarchy);

                        return request;
                    case ReportType.DisputeDetail:
                        return new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/disputes/{trb.SearchBuilder.DisputeId}",
                        };
                    case ReportType.DocumentDisputeDetail:
                        var apiRequest = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/disputes/{trb.SearchBuilder.DisputeId}/documents/{trb.SearchBuilder.DisputeDocumentId}",
                        };
                        return apiRequest;
                    case ReportType.FindDisputesPaged:
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/disputes",
                        };
                        request.AddQueryStringParam("page", trb.Page?.ToString());
                        request.AddQueryStringParam("page_size", trb.PageSize?.ToString());
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, trb.DisputeOrderBy));
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, trb.Order));
                        request.AddQueryStringParam("arn", trb.SearchBuilder.AquirerReferenceNumber);
                        request.AddQueryStringParam("brand", trb.SearchBuilder.CardBrand);
                        request.AddQueryStringParam("status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DisputeStatus));
                        request.AddQueryStringParam("stage", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DisputeStage));
                        request.AddQueryStringParam("from_stage_time_created", trb.SearchBuilder.StartStageDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_stage_time_created", trb.SearchBuilder.EndStageDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("system.mid", trb.SearchBuilder.MerchantId);
                        request.AddQueryStringParam("system.hierarchy", trb.SearchBuilder.SystemHierarchy);

                        return request;
                    case ReportType.SettlementDisputeDetail:
                        return new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/settlement/disputes/{trb.SearchBuilder.SettlementDisputeId}",
                        };
                    case ReportType.FindSettlementDisputesPaged:
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/settlement/disputes",
                        };
                        request.AddQueryStringParam("account_name", gateway.GpApiConfig.AccessTokenInfo.DataAccountName);
                        request.AddQueryStringParam("account_id", gateway.GpApiConfig.AccessTokenInfo.DataAccountID);
                        request.AddQueryStringParam("deposit_id", trb.SearchBuilder.DepositReference);
                        request.AddQueryStringParam("page", trb.Page?.ToString());
                        request.AddQueryStringParam("page_size", trb.PageSize?.ToString());
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, trb.DisputeOrderBy));
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, trb.Order));
                        request.AddQueryStringParam("arn", trb.SearchBuilder.AquirerReferenceNumber);
                        request.AddQueryStringParam("brand", trb.SearchBuilder.CardBrand);
                        request.AddQueryStringParam("STATUS", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DisputeStatus));
                        request.AddQueryStringParam("stage", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.DisputeStage));
                        request.AddQueryStringParam("from_stage_time_created", trb.SearchBuilder.StartStageDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_stage_time_created", trb.SearchBuilder.EndStageDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("from_deposit_time_created", trb.SearchBuilder.StartDepositDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_deposit_time_created", trb.SearchBuilder.EndDepositDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("system.mid", trb.SearchBuilder.MerchantId);
                        request.AddQueryStringParam("system.hierarchy", trb.SearchBuilder.SystemHierarchy);

                        return request;
                    case ReportType.StoredPaymentMethodDetail:
                        return new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/payment-methods/{trb.SearchBuilder.StoredPaymentMethodId}",
                        };
                    case ReportType.FindStoredPaymentMethodsPaged:
                        if (trb.SearchBuilder.PaymentMethod is CreditCardData) {

                            var creditCardData = trb.SearchBuilder.PaymentMethod as CreditCardData;

                            var card = new JsonDoc()
                                .Set("number", creditCardData.Number)
                                .Set("expiry_month", creditCardData.ExpMonth.HasValue ? creditCardData.ExpMonth.ToString().PadLeft(2, '0') : null)
                                .Set("expiry_year", creditCardData.ExpYear.HasValue ? creditCardData.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : null);

                            var payload = new JsonDoc()
                                .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TokenizationAccountName)
                                .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.TokenizationAccountID)
                                .Set("reference", trb.SearchBuilder.ReferenceNumber)
                                .Set("card", card != null ? card : null);

                            request = new GpApiRequest {
                                Verb = HttpMethod.Post,
                                Endpoint = $"{merchantUrl}/payment-methods/search",
                                RequestBody = payload.ToString()
                            };
                            return request;
                        }
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/payment-methods",
                        };
                        request.AddQueryStringParam("page", trb.Page?.ToString());
                        request.AddQueryStringParam("page_size", trb.PageSize?.ToString());
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, trb.StoredPaymentMethodOrderBy));
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, trb.Order));
                        request.AddQueryStringParam("id", trb.SearchBuilder.StoredPaymentMethodId);
                        request.AddQueryStringParam("number_last4", trb.SearchBuilder.CardNumberLastFour);
                        request.AddQueryStringParam("reference", trb.SearchBuilder.ReferenceNumber);
                        request.AddQueryStringParam("status", EnumConverter.GetMapping(Target.GP_API, trb.SearchBuilder.StoredPaymentMethodStatus));
                        request.AddQueryStringParam("from_time_created", trb.SearchBuilder.StartDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_time_created", trb.SearchBuilder.EndDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("from_time_last_updated", trb.SearchBuilder.StartLastUpdatedDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_time_last_updated", trb.SearchBuilder.EndLastUpdatedDate?.ToString("yyyy-MM-dd"));

                        return request;
                    case ReportType.ActionDetail:
                        return new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/actions/{trb.SearchBuilder.ActionId}",
                        };
                    case ReportType.FindActionsPaged:
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/actions",
                        };
                        request.AddQueryStringParam("page", trb.Page?.ToString());
                        request.AddQueryStringParam("page_size", trb.PageSize?.ToString());
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, trb.ActionOrderBy));
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, trb.Order));
                        request.AddQueryStringParam("id", trb.SearchBuilder.ActionId);
                        request.AddQueryStringParam("type", trb.SearchBuilder.ActionType);
                        request.AddQueryStringParam("resource", trb.SearchBuilder.Resource);
                        request.AddQueryStringParam("resource_status", trb.SearchBuilder.ResourceStatus);
                        request.AddQueryStringParam("resource_id", trb.SearchBuilder.ResourceId);
                        request.AddQueryStringParam("from_time_created", trb.SearchBuilder.StartDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_time_created", trb.SearchBuilder.EndDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("merchant_name", trb.SearchBuilder.MerchantName);
                        request.AddQueryStringParam("account_name", trb.SearchBuilder.AccountName);
                        request.AddQueryStringParam("app_name", trb.SearchBuilder.AppName);
                        request.AddQueryStringParam("version", trb.SearchBuilder.Version);
                        request.AddQueryStringParam("response_code", trb.SearchBuilder.ResponseCode);
                        request.AddQueryStringParam("http_response_code", trb.SearchBuilder.HttpResponseCode);

                        return request;

                    case ReportType.PayLinkDetail:
                        return new GpApiRequest
                        {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/links/{trb.SearchBuilder.PayLinkId}",
                        };


                    case ReportType.FindPayLinkPaged:
                        request = new GpApiRequest
                        {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/links",
                        };

                        request.AddQueryStringParam("from_time_created", trb.SearchBuilder.StartDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_time_created", trb.SearchBuilder.EndDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, trb.Order));
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, trb.ActionOrderBy));
                        request.AddQueryStringParam("status", trb.SearchBuilder.PayLinkStatus?.ToString());
                        request.AddQueryStringParam("usage_mode", trb.SearchBuilder.PaymentMethodUsageMode?.ToString());
                        request.AddQueryStringParam("name", trb.SearchBuilder.DisplayName);
                        request.AddQueryStringParam("amount", trb.SearchBuilder.Amount.ToNumericCurrencyString());
                        request.AddQueryStringParam("description", trb.SearchBuilder.Description);
                        request.AddQueryStringParam("reference", trb.SearchBuilder.ReferenceNumber);
                        request.AddQueryStringParam("country", trb.SearchBuilder.Country);
                        request.AddQueryStringParam("currency", trb.SearchBuilder.Currency);
                        request.AddQueryStringParam("expiration_date", trb.SearchBuilder.ExpirationDate?.ToString("yyyy-MM-dd"));

                        return request;
                }
            }
            else if (builder is UserReportBuilder<T> userTrb)
            {                
                switch (builder.ReportType) {
                    case ReportType.FindMerchantsPaged:
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/merchants"
                        };
                        BasicsParams(request, userTrb);
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, userTrb.Order));
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, userTrb.AccountOrderBy));
                        request.AddQueryStringParam("status", userTrb.SearchBuilder.MerchantStatus?.ToString());

                        return request;

                    case ReportType.FindAccountsPaged:
                        var endpoint = merchantUrl;
                        if(!string.IsNullOrEmpty(userTrb.SearchBuilder.MerchantId)) {
                            endpoint = $"/merchants/{userTrb.SearchBuilder.MerchantId}";
                        }
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{endpoint}/accounts",
                        };

                        BasicsParams(request, userTrb);
                        request.AddQueryStringParam("order", EnumConverter.GetMapping(Target.GP_API, userTrb.Order));
                        request.AddQueryStringParam("order_by", EnumConverter.GetMapping(Target.GP_API, userTrb.AccountOrderBy));
                        request.AddQueryStringParam("from_time_created", userTrb.SearchBuilder.StartDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("to_time_created", userTrb.SearchBuilder.EndDate?.ToString("yyyy-MM-dd"));
                        request.AddQueryStringParam("status", userTrb.SearchBuilder.AccountStatus?.ToString());
                        request.AddQueryStringParam("name", userTrb.SearchBuilder.AccountName);
                        request.AddQueryStringParam("id", userTrb.SearchBuilder.ResourceId);

                        return request;

                    case ReportType.FindAccountDetail:
                        request = new GpApiRequest {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/accounts/{builder.SearchBuilder.AccountId}",
                        };

                        if(builder.SearchBuilder.Address != null) {
                            request = new GpApiRequest {
                                Verb = HttpMethod.Get,
                                Endpoint = $"{merchantUrl}/accounts/{builder.SearchBuilder.AccountId}/addresses",
                            };
                            request.AddQueryStringParam("postal_code", userTrb.SearchBuilder.Address.PostalCode);
                            request.AddQueryStringParam("line_1", userTrb.SearchBuilder.Address.StreetAddress1);
                            request.AddQueryStringParam("line_2", userTrb.SearchBuilder.Address.StreetAddress2);
                        }

                        return request;
                }
                   
            }                
            
            return null;
        }

        private static void BasicsParams<T>(GpApiRequest request, UserReportBuilder<T> userTrb) where T : class
        {
            request.AddQueryStringParam("page", userTrb.Page.ToString());
            request.AddQueryStringParam("page_size", userTrb.PageSize.ToString());
        }
    }
}
