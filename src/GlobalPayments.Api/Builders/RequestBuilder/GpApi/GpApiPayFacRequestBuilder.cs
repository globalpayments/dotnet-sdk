using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Logging;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Builders.RequestBuilder.GpApi
{
    internal class GpApiPayFacRequestBuilder<T> : IRequestBuilder<PayFacBuilder<T>> where T : class
    {
        private PayFacBuilder<T> _builder { get; set; }
        private readonly MaskedValueCollection _maskedValueCollection = new MaskedValueCollection();
        private Dictionary<string, string> MaskedValues;

        public Request BuildRequest(PayFacBuilder<T> builder, GpApiConnector gateway)
        {
            _builder = builder;
            var merchantManagementUrl = GpApiRequest.GetMerchantManagementUrl(gateway.GpApiConfig);
            Validate(builder.TransactionType, gateway);
            switch (builder.TransactionType)
            {
                case TransactionType.Create:
                    if (builder.TransactionModifier == TransactionModifier.Merchant)
                    {
                        var data = BuildCreateMerchantRequest();

                        return new Request
                        {
                            Verb = HttpMethod.Post,
                            Endpoint = merchantManagementUrl,
                            RequestBody = data.ToString(),
                            MaskedValues = MaskedValues,
                        };
                    }
                    break;
                case TransactionType.Edit:
                    if (builder.TransactionModifier == TransactionModifier.Merchant)
                    {
                        return new Request
                        {
                            Verb = new HttpMethod("PATCH"),
                            Endpoint = $"{merchantManagementUrl}/{_builder.UserReference.UserId}",
                            RequestBody = BuildEditMerchantRequest().ToString(),
                        };
                    }
                    else if (builder.TransactionModifier == TransactionModifier.Account)
                    {
                        var dataRequest = BuildEditAccountRequest();

                        var endpoint = GpApiRequest.GetMerchantUrl(gateway.GpApiConfig);
                        if (!string.IsNullOrEmpty(builder.UserReference?.UserId))
                        {
                            endpoint = GpApiRequest.GetMerchantUrl(gateway.GpApiConfig, builder.UserReference.UserId);
                        }

                        return new Request
                        {
                            Verb = new HttpMethod("PATCH"),
                            Endpoint = $"{endpoint}{GpApiRequest.ACCOUNTS_ENDPOINT}/{_builder.AccountNumber}",
                            RequestBody = dataRequest.ToString(),
                            MaskedValues = _maskedValueCollection.ToDictionary(),
                        };
                    }
                    break;
                case TransactionType.Fetch:
                    if (builder.TransactionModifier == TransactionModifier.Merchant)
                    {
                        return new Request
                        {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantManagementUrl}/{_builder.UserReference.UserId}",
                        };
                    }
                    break;
                case TransactionType.AddFunds:
                    var dataFunds = new JsonDoc();
                    dataFunds.Set("account_id", builder.AccountNumber)
                        .Set("type", builder.PaymentMethodType.ToString() ?? null)
                        .Set("amount", builder.Amount)
                        .Set("currency", builder.Currency ?? null)
                        .Set("payment_method", builder.PaymentMethodName.ToString() ?? null)
                        .Set("reference", builder.ClientTransactionId ?? GenerationUtils.GenerateOrderId());
                    return new Request
                    {
                        Verb = HttpMethod.Post,
                        Endpoint = $"{merchantManagementUrl}/{_builder.UserReference.UserId}/settlement/funds",
                        RequestBody = dataFunds.ToString(),
                    };
                    
                    break;
                case TransactionType.UploadDocument:
                    var requestData = new JsonDoc();
                    requestData.Set("function", builder.DocumentUploadData.DocCategory.ToString())
                        .Set("b64_content", builder.DocumentUploadData.Document)
                        .Set("format", builder.DocumentUploadData.DocType.ToString());
                    return new Request
                    {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantManagementUrl}/{_builder.UserReference.UserId}/documents",
                            RequestBody = requestData.ToString()
                    };                    
                    break;
                default:
                    break;
            }

            return null;
        }

        private void Validate(TransactionType transactionType, GpApiConnector gateway)
        {
            string errorMsg = string.Empty;
            switch (transactionType) {
                case TransactionType.AddFunds:
                    if (string.IsNullOrEmpty(gateway.GpApiConfig.MerchantId) && string.IsNullOrEmpty(_builder.UserReference?.UserId)) {
                        errorMsg = "property UserId or config MerchantId cannot be null for this transactionType";
                    }
                    break;
                default:
                break;
            }

            if (!string.IsNullOrEmpty(errorMsg)) {
                throw new GatewayException(errorMsg);
            }
        }

        private Dictionary<string, object> MapAddress(Address address, string countryCodeType = null, string functionKey = null)
        {
            var countryCode = string.Empty;
            switch (countryCodeType)
            {
                case "alpha2":
                    countryCode = CountryUtils.GetCountryCodeByCountry(address.CountryCode);
                    break;
                default:
                    countryCode = address.CountryCode;
                    break;
            }
            var item = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(functionKey))
                item.Add("functions", new string[] { functionKey });
            if (!string.IsNullOrEmpty(address?.StreetAddress1))
                item.Add("line_1", address.StreetAddress1);
            if (!string.IsNullOrEmpty(address?.StreetAddress2))
                item.Add("line_2", address.StreetAddress2);
            if (!string.IsNullOrEmpty(address?.StreetAddress3))
                item.Add("line_3", address.StreetAddress3);
            if (!string.IsNullOrEmpty(address?.City))
                item.Add("city", address.City);
            if (!string.IsNullOrEmpty(address?.PostalCode))
                item.Add("postal_code", address.PostalCode);
            if (!string.IsNullOrEmpty(address?.State))
                item.Add("state", address.State);

            item.Add("country", countryCode);

            return item;
        }

        private JsonDoc SetMerchantInfo()
        {
            if (_builder.UserPersonalData == null)
            {
                return new JsonDoc();
            }
            var merchantData = _builder.UserPersonalData;
            var data = new JsonDoc()
                        .Set("name", merchantData.UserName)
                        .Set("legal_name", merchantData.LegalName)
                        .Set("dba", merchantData.DBA)
                        .Set("merchant_category_code", merchantData.MerchantCategoryCode)
                        .Set("website", merchantData.Website)
                        .Set("currency", merchantData.CurrencyCode)
                        .Set("tax_id_reference", merchantData.TaxIdReference)
                        .Set("notification_email", merchantData.NotificationEmail)
                        .Set("status", _builder.UserReference?.UserStatus.ToString() ?? null);

            var notifications = new JsonDoc()
               .Set("status_url", merchantData.NotificationStatusUrl);

            if (notifications.HasKeys())
            {
                data.Set("notifications", notifications);
            }

            return data;
        }

        private JsonDoc BuildCreateMerchantRequest()
        {
            var merchantData = _builder.UserPersonalData;
            var data = SetMerchantInfo();
            data.Set("pricing_profile", merchantData.Tier)
                .Set("description", _builder.Description)
                .Set("type", merchantData.Type.ToString())
                .Set("addresses", SetAddressList())
                .Set("payment_processing_statistics", SetPaymentStatistics());

            data.Set("payment_methods", SetPaymentMethod())
                .Set("persons", SetPersonList())
                .Set("products", _builder.ProductData?.Count > 0 ? SetProductList(_builder.ProductData) : null);

            return data;
        }

        private JsonDoc SetPaymentStatistics()
        {
            if (_builder.PaymentStatistics == null)
            {
                return null;
            }

            return new JsonDoc()
                .Set("total_monthly_sales_amount", _builder.PaymentStatistics.TotalMonthlySalesAmount.ToNumericCurrencyString())
                .Set("average_ticket_sales_amount", _builder.PaymentStatistics.AverageTicketSalesAmount.ToNumericCurrencyString())
                .Set("highest_ticket_sales_amount", _builder.PaymentStatistics.HighestTicketSalesAmount.ToNumericCurrencyString());
        }

        private List<Dictionary<string, object>> SetPersonList(string type = null)
        {
            if (_builder.PersonsData?.Count == 0 || _builder.PersonsData == null)
            {
                return null;
            }
            var personInfo = new List<Dictionary<string, object>>();
            foreach (var person in _builder.PersonsData)
            {
                var item = new Dictionary<string, object>();
                item.Add("functions", new string[] { person.Functions.ToString() });
                item.Add("first_name", person.FirstName);
                item.Add("middle_name", person.MiddleName);
                item.Add("last_name", person.LastName);
                item.Add("email", person.Email);
                item.Add("date_of_birth", person.DateOfBirth ?? null);
                item.Add("national_id_reference", person.NationalIdReference);
                item.Add("equity_percentage", person.EquityPercentage);
                item.Add("job_title", person.JobTitle);


                if (person.Address != null && type == null)
                {
                    item.Add("address", MapAddress(person.Address));
                }
                if (person.HomePhone != null)
                {
                    var contactPhone = new Dictionary<string, object>();
                    contactPhone.Add("country_code", person.HomePhone.CountryCode);
                    contactPhone.Add("subscriber_number", person.HomePhone.Number);

                    item.Add("contact_phone", contactPhone);
                }
                if (person.WorkPhone != null)
                {
                    var workPhone = new Dictionary<string, object>();
                    workPhone.Add("country_code", person.WorkPhone.CountryCode);
                    workPhone.Add("subscriber_number", person.WorkPhone.Number);

                    item.Add("work_phone", workPhone);
                }
                personInfo.Add(item);
            }

            return personInfo;
        }

        private Dictionary<string, object> SetBankTransferInfo(BankAccountData bankAccountData)
        {
            var data = new Dictionary<string, object>();
            data.Add("account_holder_type", bankAccountData?.AccountOwnershipType);
            data.Add("account_number", bankAccountData?.AccountNumber);
            data.Add("account_type", bankAccountData?.AccountType);

            var bank = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(bankAccountData?.BankName))
                bank.Add("name", bankAccountData?.BankName);
            if (!string.IsNullOrEmpty(bankAccountData?.RoutingNumber))
                bank.Add("code", bankAccountData?.RoutingNumber); //@TODO confirmantion from GP-API team

            bank.Add("international_code", ""); //@TODO

            bank.Add("address", (bankAccountData.BankAddress != null) ? MapAddress(bankAccountData.BankAddress, "alpha2") : null);

            data.Add("bank", bank);

            return data;
        }

        private Dictionary<string, object> SetCreditCardInfo(CreditCardData creditCardInformation)
        {
            var item = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(creditCardInformation?.CardHolderName))
                item.Add("name", creditCardInformation?.CardHolderName);
            item.Add("number", creditCardInformation?.Number);
            item.Add("expiry_month", creditCardInformation?.ExpMonth);
            item.Add("expiry_year", creditCardInformation?.ExpYear);

            var maskedValue = new Dictionary<string, string>();
            maskedValue.Add("payment_methods;list.card.expiry_month", creditCardInformation?.ExpMonth.ToString().PadLeft(2, '0') ?? string.Empty);
            maskedValue.Add("payment_methods;list.card.expiry_year", creditCardInformation?.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) ?? string.Empty);            

            MaskedValues = _maskedValueCollection.HideValues(maskedValue);
            MaskedValues = _maskedValueCollection.HideValue("payment_methods;list.card.number", creditCardInformation?.Number, 4, 6);

            return item;
        }

        private List<Dictionary<string, object>> SetProductList(List<Product> productData)
        {
            var products = new List<Dictionary<string, object>>();

            foreach (var product in productData)
            {
                var deviceInfo = new Dictionary<string, object>();
                if (product.ProductId.Contains("_CP-"))
                {
                    deviceInfo.Add("quantity", 1);
                }
                var item = new Dictionary<string, object>();
                item.Add("device", deviceInfo.Count > 0 ? deviceInfo : null);
                item.Add("id", product.ProductId);
                products.Add(item);
            }
            return products;
        }

        private List<Dictionary<string, object>> SetPaymentMethod()
        {
            if (_builder.PaymentMethodsFunctions == null)
            {
                return null;
            }
            var paymentMethods = new List<Dictionary<string, object>>();
            var item1 = new Dictionary<string, object>();
            item1.Add("functions", new string[] { _builder.PaymentMethodsFunctions?[_builder.CreditCardInformation.GetType().Name].ToString() } ?? null);
            item1.Add("card", SetCreditCardInfo(_builder.CreditCardInformation));
            paymentMethods.Add(item1);

            var item2 = new Dictionary<string, object>();
            item2.Add("functions", new string[] { _builder.PaymentMethodsFunctions?[_builder.BankAccountData.GetType().Name].ToString() } ?? null);
            item2.Add("name", _builder.BankAccountData?.AccountHolderName);
            item2.Add("bank_transfer", SetBankTransferInfo(_builder.BankAccountData));
            paymentMethods.Add(item2);

            return paymentMethods;
        }

        private List<Dictionary<string, object>> SetAddressList()
        {
            if (_builder.UserPersonalData == null)
            {
                return null;
            }
            var merchantData = _builder.UserPersonalData;
            var addressList = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(merchantData?.UserAddress?.StreetAddress1))
            {
                addressList.Add(AddressType.Business.ToString(), merchantData.UserAddress);
            }
            if (!string.IsNullOrEmpty(merchantData?.MailingAddress.StreetAddress1))
            {
                addressList.Add(AddressType.Shipping.ToString(), merchantData.MailingAddress);
            }
            var addresses = new List<Dictionary<string, object>>();
            foreach (KeyValuePair<string, object> address in addressList)
            {
                var item = new Dictionary<string, object>();
                var dataAddress = ((Address)address.Value);
                addresses.Add(MapAddress(dataAddress, "alpha2", address.Key));
            }

            return addresses;
        }

        /// <summary>
        /// Builds optional account edits and request-scoped card masks.
        /// </summary>
        /// <returns>The PATCH request body.</returns>
        private JsonDoc BuildEditAccountRequest() {
            var data = _builder.AccountUpdateData;
            var request = new JsonDoc();
            if (data?.Status != null) {
                request.Set("status", data.Status.Value.ToString());
            }
            var payer = new JsonDoc();
            var paymentMethod = new JsonDoc().Set("reference", data?.PaymentMethodReference);
            var card = _builder.CreditCardInformation;
            if (card != null) {
                var month = card.ExpMonth.HasValue ? card.ExpMonth.ToString().PadLeft(2, '0') : null;
                var year = card.ExpYear.HasValue ? card.ExpYear.ToString().PadLeft(4, '0').Substring(2, 2) : null;
                paymentMethod.Set("name", card.CardHolderName);
                paymentMethod.Set("card", new JsonDoc()
                    .Set("name", card.CardHolderName)
                    .Set("number", card.Number)
                    .Set("expiry_month", month)
                    .Set("expiry_year", year)
                    .Set("cvv", card.Cvn));
                _maskedValueCollection.HideValue("payer.payment_method.card.number", card.Number, 4, 6);
                _maskedValueCollection.HideValue("payer.payment_method.card.expiry_month", month);
                _maskedValueCollection.HideValue("payer.payment_method.card.expiry_year", year);
                _maskedValueCollection.HideValue("payer.payment_method.card.cvv", card.Cvn);
            }
            if (paymentMethod.HasKeys()) {
                payer.Set("payment_method", paymentMethod);
            }
            Address billing;
            if (_builder.Addresses != null && _builder.Addresses.TryGetValue(AddressType.Billing, out billing) && billing != null) {
                // Retain the existing layout used by the GP-API payout-card example.
                payer.Set("billing_address", new JsonDoc()
                    .Set("line_1", billing.StreetAddress1)
                    .Set("line_2", billing.StreetAddress2)
                    .Set("line_3", billing.StreetAddress3)
                    .Set("city", billing.City)
                    .Set("state", billing.State)
                    .Set("postal_code", billing.PostalCode)
                    .Set("country", CountryUtils.GetCountryCodeByCountry(billing.CountryCode)));
            }
            if (payer.HasKeys()) {
                request.Set("payer", payer);
            }
            request.Set("card_replacement_reason", data?.CardReplacementReason)
                .Set("status_change_reason", data?.StatusChangeReason)
                .Set("capabilities", data?.Capabilities);
            if (data?.StatusUrl != null) {
                request.Set("notifications", new JsonDoc().Set("status_url", data.StatusUrl));
            }
            var limits = data?.ProcessingLimits;
            if (limits != null) {
                request.Set("processing_limits", new JsonDoc()
                    .Set("allowed_negative_balance_amount", limits.AllowedNegativeBalanceAmount.ToNumericCurrencyString(null))
                    .Set("card_transaction_limit_amount", limits.CardTransactionLimitAmount.ToNumericCurrencyString(null))
                    .Set("card_monthly_limit_amount", limits.CardMonthlyLimitAmount.ToNumericCurrencyString(null))
                    .Set("bank_transfer_transaction_limit_amount", limits.BankTransferTransactionLimitAmount.ToNumericCurrencyString(null))
                    .Set("bank_transfer_monthly_limit_amount", limits.BankTransferMonthlyLimitAmount.ToNumericCurrencyString(null))
                    .Set("disable_payout_card_limit_percentage", limits.DisablePayoutCardLimitPercentage?.ToString(CultureInfo.InvariantCulture))
                    .Set("disable_payout_bank_transfer_limit_percentage", limits.DisablePayoutBankTransferLimitPercentage?.ToString(CultureInfo.InvariantCulture)));
            }
            return request;
        }

        private JsonDoc BuildEditMerchantRequest()
        {
            var requestBody = SetMerchantInfo();
            requestBody
                //.Set("description", _builder.Description)
                .Set("status_change_reason", _builder.StatusChangeReason?.ToString() ?? null)
                .Set("addresses", SetAddressList() ?? null)
                .Set("persons", SetPersonList("edit"))
                .Set("payment_processing_statistics", SetPaymentStatistics())
                .Set("payment_methods", SetPaymentMethod());

            return requestBody;
        }
    }
}
