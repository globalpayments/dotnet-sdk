using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GlobalPayments.Api.Builders.RequestBuilder.GpApi {
    internal class GpApiInstallmentRequestBuilder : IRequestBuilder<InstallmentBuilder> {
        private static Dictionary<string, string> _maskedValues;
        private static InstallmentBuilder _builder;
        public Request BuildRequest(InstallmentBuilder builder, GpApiConnector gateway) {
            _builder = builder;

            // GET installment by ID
            if (builder.InstallmentId != null && builder.Entity == null) {
                return new Request {
                    Verb = HttpMethod.Get,
                    Endpoint = $"{GpApiRequest.INSTALLMENT_ENDPOINT}/{builder.InstallmentId}",
                };
            }

            JsonDoc data = PrepareInstallmentRequest(builder, gateway);

            return new Request
            {
                Verb = HttpMethod.Post,
                Endpoint = $"{GpApiRequest.INSTALLMENT_ENDPOINT}",
                RequestBody = data.ToString(),
            };
        }

        private JsonDoc PrepareInstallmentRequest(InstallmentBuilder builder, GpApiConnector gateway) {
           
            var installment = _builder.Entity as Installment;
            var requestData = new JsonDoc()               
                .Set("account_name", gateway.GpApiConfig.AccessTokenInfo.TransactionProcessingAccountName)
                .Set("amount", StringUtils.ToNumeric(installment.Amount))
                .Set("channel", EnumConverter.GetMapping(Target.GP_API, gateway.GpApiConfig.Channel))
                .Set("currency", installment.Currency)
                .Set("country", gateway.GpApiConfig.Country)
                .Set("reference", installment.Reference)
                .Set("program", installment.Program);

            if (installment.CardDetails == null) {
                throw new ApiException("Card details are mandatory and cannot be null");
            }

            var cardData = new JsonDoc()
                .Set("number", installment.CardDetails.Number)
                .Set("expiry_month", installment.CardDetails.ExpMonth?.ToString().PadLeft(2, '0'))
                .Set("expiry_year", installment.CardDetails.ExpYear?.ToString().PadLeft(4, '0').Substring(2, 2))
                .Set("brand", installment.CardDetails.CardType);

            JsonDoc paymentMethodData = new JsonDoc()
                    .Set("entry_mode", installment.EntryMode)
                    .Set("card", cardData);
            requestData.Set("payment_method", paymentMethodData);

            // Add Visa installment specific fields
            if (installment.FundingMode.HasValue) {
                requestData.Set("funding_mode", EnumConverter.GetMapping(Target.GP_API, installment.FundingMode.Value));
            }

            if (installment.InstallmentTerms != null) {
                var termsData = new JsonDoc();
                if (!string.IsNullOrEmpty(installment.InstallmentTerms.TimeUnit)) {
                    termsData.Set("time_unit", installment.InstallmentTerms.TimeUnit);
                }
                if (installment.InstallmentTerms.MaxTimeUnitNumber.HasValue) {
                    termsData.Set("max_time_unit_number", installment.InstallmentTerms.MaxTimeUnitNumber.Value);
                }
                if (installment.InstallmentTerms.MaxAmount.HasValue) {
                    termsData.Set("max_amount", installment.InstallmentTerms.MaxAmount.Value);
                }
                if (termsData.HasKeys()) {
                    requestData.Set("terms", termsData);
                }
            }

            if (installment.EligiblePlans.HasValue) {
                requestData.Set("eligible_plans", EnumConverter.GetMapping(Target.GP_API, installment.EligiblePlans.Value));
            }

            return requestData;
        }
    }
}
