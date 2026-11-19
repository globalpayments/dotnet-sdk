using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Entities.Reporting;
using GlobalPayments.Api.Mapping;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Reflection;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiMappingTest {
        private const string AccountResponseJson = @"{
            'id':'FMA_contract','name':'Fund management','type':'FUND_MANAGEMENT','status':'ACTIVE',
            'permissions':['ACC_GET'],'countries':['US'],'currencies':['USD'],'channels':['CNP'],
            'payment_methods':['CARD','BANK_TRANSFER'],'email_id':'accounts@example.com',
            'merchant_id':'MER_contract','merchant_name':'Contract merchant',
            'time_created':'2026-01-02T03:04:05.678Z','time_last_updated':'2026-01-02T03:04:05.678Z',
            'card_replacment_reason':'LOST','capabilities':['PAY_IN_ENABLE','PAY_OUT_DISABLE'],
            'configurations':[{'id':'RTC_contract','status':'ACTIVE','serial_number':'serial_contract',
                'actions':['AUTHORIZE','LINK_LIST'],'countries':['US'],'currencies':['USD'],
                'channels':['CNP'],'payment_methods':['CARD','BANK_TRANSFER'],
                'platform':{'id':'PFC_configuration','reference':'PTF_configuration','name':'TRUUST',
                    'platform_name':'Configuration platform','platform_mid':'configuration_mid',
                    'country':'GB','email':'platform@example.com','authentication':'ENABLE',
                    'masked_secretkey_last4':'XXXXXXXX1234'}}],
            'addresses':[{'line_1':'Line 1','line_2':'Line 2','line_3':'Line 3','city':'Atlanta','state':'GA','postal_code':'30346','country':'US'}],
            'action':{'id':'ACT_contract','type':'ACCOUNTS_SINGLE','time_created':'2026-01-02T03:04:05.678Z',
                'result_code':'SUCCESS','app_id':'APP_contract','app_name':'Contract app'}}";
        private const string AuthenticationResponseJson = @"{
            'id':'AUT_contract','time_created':'2026-01-02T03:04:05.678Z',
            'time_last_updated':'2026-01-03T04:05:06.789Z','transaction_type':'SALE','status':'AVAILABLE',
            'channel':'CNP','amount':'1234','currency':'GBP','country':'GB','source':'BROWSER',
            'merchant_id':'MER_contract','merchant_name':'Contract merchant','account_id':'TRA_contract',
            'account_name':'transaction_processing','reference':'authentication_reference',
            'payment_method':{'result':'00','message':'APPROVED','entry_mode':'ECOM','fingerprint':'fingerprint',
                'fingerprint_presence_indicator':'NEW','name':'Test card',
                'card':{'funding':'CREDIT','brand':'VISA','authcode':'AUTH','brand_reference':'BRAND_REF',
                    'masked_number_first6last4':'411111xxxx1111','cvv_indicator':'PRESENT','cvv_result':'MATCHED',
                    'avs_address_result':'MATCHED','avs_postal_code_result':'NOT_CHECKED'},
                'three_ds':{'acs_trans_ref':'ACS_PAYMENT','ds_trans_ref':'DS_PAYMENT',
                    'server_trans_ref':'SERVER_PAYMENT','liability_shift':'YES','value':'CAVV_PAYMENT',
                    'eci':'05','status':'AUTHENTICATION_SUCCESSFUL','status_reason':'HIGH_CONFIDENCE',
                    'message_category':'PAYMENT_AUTHENTICATION','message_version':'2.2.0','challenge_status':'NOT_MANDATED'}},
            'three_ds':{'acs_trans_ref':'ACS_AUTH','acs_reference_number':'ACS_REF','ds_trans_ref':'DS_AUTH',
                'server_trans_ref':'SERVER_AUTH','liability_shift':'YES','value':'CAVV_AUTH','eci':'05',
                'status':'AUTHENTICATION_SUCCESSFUL','status_reason':'HIGH_CONFIDENCE',
                'message_category':'PAYMENT_AUTHENTICATION','message_version':'2.2.0','challenge_status':'NOT_MANDATED',
                'ds_protocol_version_start':'2.1.0','ds_protocol_version_end':'2.2.0',
                'acs_protocol_version_start':'2.1.0','acs_protocol_version_end':'2.2.0',
                'acs_info_indicator':['AUTHENTICATION_AVAILABLE_AT_ACS'],'enrolled_status':'ENROLLED',
                'challenge_model':'STATIC_CHALLENGE','session_data_field_name':'threeDSSessionData',
                'message_type':'CREQ','challenge_value':'challenge-value','redirect_url':'https://acs.example/redirect',
                'acs_challenge_request_url':'https://acs.example/challenge','method_url':'https://acs.example/method',
                'method_data':{'three_ds_server_trans_id':'METHOD_TRANS','three_ds_method_return_url':'https://example.com/method-return',
                    'encoded_method_data':'encoded-method-data'},
                'mobile_data':{'acs_signed_content':'signed-content',
                    'acs_rendering_type':{'acs_interface':'NATIVE','acs_ui_template':'SINGLE_SELECT'}}},
            'notifications':{'challenge_return_url':'https://example.com/challenge-return'},
            'action_create_id':'ACT_CREATE','system':{'mid':'MID_contract','tid':'TID_contract','name':'System'},
            'action':{'id':'ACT_auth','type':'CHECK_AVAILABILITY','time_created':'2026-01-02T03:04:05.678Z',
                'result_code':'SUCCESS','app_id':'APP_contract','app_name':'Contract app'}}";

        /// <summary>
        /// Serializes a status-only edit without adding unrelated account fields.
        /// </summary>
        [DataTestMethod]
        [DataRow(MerchantAccountStatus.ACTIVE, "ACTIVE")]
        [DataRow(MerchantAccountStatus.INACTIVE, "INACTIVE")]
        public void BuildEditAccountRequest_WithStatus_MapsOnlyStatus(MerchantAccountStatus status, string expectedStatus) {
            var builder = new PayFacService().EditAccount()
                .WithAccountNumber("FMA_contract")
                .WithAccountUpdateData(new AccountUpdateData { Status = status });

            var request = BuildEditAccountRequest(builder);

            Assert.IsNotNull(request);
            Assert.AreEqual("PATCH", request.Verb.Method);
            Assert.AreEqual("/accounts/FMA_contract", request.Endpoint);
            Assert.AreEqual("{\"status\":\"" + expectedStatus + "\"}", request.RequestBody);
        }

        /// <summary>
        /// Leaves status out of edits that do not request a status change.
        /// </summary>
        [TestMethod]
        public void BuildEditAccountRequest_WithoutStatus_OmitsStatus() {
            var builder = new PayFacService().EditAccount()
                .WithAccountNumber("FMA_contract")
                .WithAccountUpdateData(new AccountUpdateData { StatusChangeReason = "ReadyToProcess" });

            var request = BuildEditAccountRequest(builder);

            Assert.IsNotNull(request);
            Assert.AreEqual("{\"status_change_reason\":\"ReadyToProcess\"}", request.RequestBody);
        }

        /// <summary>
        /// Builds every optional account edit field without sending a gateway request.
        /// </summary>
        [TestMethod]
        public void BuildEditAccountRequest_WithOptionalFields_MapsAllFields() {
            var builder = new PayFacService().EditAccount()
                .WithAccountNumber("FMA_contract")
                .WithAccountUpdateData(new AccountUpdateData {
                    PaymentMethodReference = "payment_reference",
                    CardReplacementReason = "LOST",
                    StatusChangeReason = "ReadyToProcess",
                    StatusUrl = "https://example.com/account-status",
                    Capabilities = new List<string> { "PAY_IN_ENABLE" },
                    ProcessingLimits = new AccountProcessingLimits {
                        AllowedNegativeBalanceAmount = 0m,
                        CardTransactionLimitAmount = 101.01m,
                        CardMonthlyLimitAmount = 202.02m,
                        BankTransferTransactionLimitAmount = 303.03m,
                        BankTransferMonthlyLimitAmount = 404.04m,
                        DisablePayoutCardLimitPercentage = 12.5m,
                        DisablePayoutBankTransferLimitPercentage = 25.5m
                    }
                });
            var oldCulture = CultureInfo.CurrentCulture;
            try {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                var request = BuildEditAccountRequest(builder);

                Assert.IsNotNull(request);
                Assert.AreEqual("PATCH", request.Verb.Method);
                Assert.AreEqual("/accounts/FMA_contract", request.Endpoint);
                var body = JsonDoc.Parse(request.RequestBody);
                Assert.IsNotNull(body);
                Assert.IsNull(body.GetValue<string>("status"));
                Assert.AreEqual("payment_reference", body.Get("payer")?.Get("payment_method")?.GetValue<string>("reference"));
                Assert.AreEqual("LOST", body.GetValue<string>("card_replacement_reason"));
                Assert.AreEqual("ReadyToProcess", body.GetValue<string>("status_change_reason"));
                Assert.AreEqual("https://example.com/account-status", body.Get("notifications")?.GetValue<string>("status_url"));
                var capabilities = body.GetArray<string>("capabilities");
                Assert.IsNotNull(capabilities);
                CollectionAssert.AreEqual(new List<string> { "PAY_IN_ENABLE" }, new List<string>(capabilities));
                var limits = body.Get("processing_limits");
                Assert.IsNotNull(limits);
                Assert.AreEqual("0", limits.GetValue<string>("allowed_negative_balance_amount"));
                Assert.AreEqual("10101", limits.GetValue<string>("card_transaction_limit_amount"));
                Assert.AreEqual("20202", limits.GetValue<string>("card_monthly_limit_amount"));
                Assert.AreEqual("30303", limits.GetValue<string>("bank_transfer_transaction_limit_amount"));
                Assert.AreEqual("40404", limits.GetValue<string>("bank_transfer_monthly_limit_amount"));
                Assert.AreEqual("12.5", limits.GetValue<string>("disable_payout_card_limit_percentage"));
                Assert.AreEqual("25.5", limits.GetValue<string>("disable_payout_bank_transfer_limit_percentage"));
            }
            finally {
                CultureInfo.CurrentCulture = oldCulture;
            }
        }

        /// <summary>
        /// Preserves payout-card fields, billing address, and request-scoped sensitive-field masks.
        /// </summary>
        [TestMethod]
        public void BuildEditAccountRequest_WithPayoutCard_MapsFieldsAndMasks() {
            var card = TestCards.VisaManual();
            card.CardHolderName = "Account holder";
            var builder = new PayFacService().EditAccount()
                .WithAccountNumber("FMA_contract")
                .WithCreditCardData(card)
                .WithAddress(new Address {
                    StreetAddress1 = "Line 1",
                    StreetAddress2 = "Line 2",
                    StreetAddress3 = "Line 3",
                    City = "Atlanta",
                    State = "GA",
                    PostalCode = "30346",
                    Country = "US"
                });

            var request = BuildEditAccountRequest(builder);

            Assert.IsNotNull(request);
            var body = JsonDoc.Parse(request.RequestBody);
            Assert.IsNotNull(body);
            var payer = body.Get("payer");
            Assert.IsNotNull(payer);
            var paymentMethod = payer.Get("payment_method");
            Assert.IsNotNull(paymentMethod);
            Assert.AreEqual(card.CardHolderName, paymentMethod.GetValue<string>("name"));
            var mappedCard = paymentMethod.Get("card");
            Assert.IsNotNull(mappedCard);
            Assert.AreEqual(card.CardHolderName, mappedCard.GetValue<string>("name"));
            Assert.IsTrue(card.Number == mappedCard.GetValue<string>("number"), "The payout card number must be preserved.");
            Assert.AreEqual(card.ExpMonth.Value.ToString("00", CultureInfo.InvariantCulture), mappedCard.GetValue<string>("expiry_month"));
            Assert.AreEqual(card.ExpYear.Value.ToString("0000", CultureInfo.InvariantCulture).Substring(2), mappedCard.GetValue<string>("expiry_year"));
            Assert.IsTrue(card.Cvn == mappedCard.GetValue<string>("cvv"), "The payout card verification value must be preserved.");
            var billing = payer.Get("billing_address");
            Assert.IsNotNull(billing);
            Assert.AreEqual("Line 1", billing.GetValue<string>("line_1"));
            Assert.AreEqual("Line 2", billing.GetValue<string>("line_2"));
            Assert.AreEqual("Line 3", billing.GetValue<string>("line_3"));
            Assert.AreEqual("Atlanta", billing.GetValue<string>("city"));
            Assert.AreEqual("GA", billing.GetValue<string>("state"));
            Assert.AreEqual("30346", billing.GetValue<string>("postal_code"));
            Assert.AreEqual("US", billing.GetValue<string>("country"));
            var masksProperty = typeof(Request).GetProperty("MaskedValues", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(masksProperty);
            var masks = (Dictionary<string, string>)masksProperty.GetValue(request);
            Assert.IsNotNull(masks);
            Assert.AreEqual(4, masks.Count);
            Assert.AreEqual(card.Number.Substring(0, 6) + new string('X', card.Number.Length - 10) + card.Number.Substring(card.Number.Length - 4), masks["payer.payment_method.card.number"]);
            Assert.AreEqual("XX", masks["payer.payment_method.card.expiry_month"]);
            Assert.AreEqual("XX", masks["payer.payment_method.card.expiry_year"]);
            Assert.AreEqual(new string('X', card.Cvn.Length), masks["payer.payment_method.card.cvv"]);
        }

        /// <summary>
        /// Invokes the internal PayFac request builder without exposing it as public SDK API or making HTTP calls.
        /// </summary>
        /// <param name="builder">The account edit request.</param>
        /// <returns>The constructed PATCH request.</returns>
        private static Request BuildEditAccountRequest(PayFacBuilder<Transaction> builder) {
            var assembly = typeof(GpApiMapping).Assembly;
            var requestBuilderType = assembly.GetType("GlobalPayments.Api.Builders.RequestBuilder.GpApi.GpApiPayFacRequestBuilder`1");
            Assert.IsNotNull(requestBuilderType);
            requestBuilderType = requestBuilderType.MakeGenericType(typeof(Transaction));
            var requestBuilder = Activator.CreateInstance(requestBuilderType);
            var connectorType = assembly.GetType("GlobalPayments.Api.Gateways.GpApiConnector");
            Assert.IsNotNull(connectorType);
            var connector = Activator.CreateInstance(connectorType, new object[] { new GpApiConfig() });
            var method = requestBuilderType.GetMethod("BuildRequest");
            Assert.IsNotNull(method);
            return (Request)method.Invoke(requestBuilder, new object[] { builder, connector });
        }

        /// <summary>
        /// Maps every common account response field directly from a fixed JSON response.
        /// </summary>
        [TestMethod]
        public void MapReportResponse_FindAccountDetail_MapsAllFields() {
            // Act
            var account = GpApiMapping.MapReportResponse<MerchantAccountSummary>(AccountResponseJson, ReportType.FindAccountDetail);

            // Assert
            AssertMappedAccount(account);
        }

        /// <summary>
        /// Maps account paging, filters, merchant metadata, action, and account records.
        /// </summary>
        [TestMethod]
        public void MapReportResponse_FindAccountsPaged_MapsEnvelopeAndAccountRecords() {
            // Arrange
            var rawJson = @"{
                'total_record_count':11,'current_page_size':1,
                'merchant_id':'MER_contract','merchant_name':'Contract merchant',
                'paging':{'page':2,'page_size':10,'order':'ASC','order_by':'TIME_CREATED'},
                'filter':{'from_time_created':'2026-01-01','to_time_created':'2026-01-31'},
                'action':{'id':'ACT_contract','type':'ACCOUNTS_LIST','time_created':'2026-01-02T03:04:05.678Z',
                    'result_code':'SUCCESS','app_id':'APP_contract','app_name':'Contract app'},
                'accounts':[" + AccountResponseJson + "]}";

            // Act
            var page = GpApiMapping.MapReportResponse<PagedResult<MerchantAccountSummary>>(rawJson, ReportType.FindAccountsPaged);

            // Assert
            Assert.IsNotNull(page);
            Assert.AreEqual(11, page.TotalRecordCount);
            Assert.AreEqual(1, page.CurrentPageSize);
            Assert.AreEqual(2, page.Page);
            Assert.AreEqual(10, page.PageSize);
            Assert.AreEqual("ASC", page.Order);
            Assert.AreEqual("TIME_CREATED", page.OrderBy);
            Assert.AreEqual("MER_contract", page.MerchantId);
            Assert.AreEqual("Contract merchant", page.MerchantName);
            Assert.AreEqual(new DateTime(2026, 1, 1), page.FromTimeCreated);
            Assert.AreEqual(new DateTime(2026, 1, 31), page.ToTimeCreated);
            AssertMappedAccountAction(page.Action, "ACCOUNTS_LIST");
            Assert.IsNotNull(page.Results);
            Assert.AreEqual(1, page.Results.Count);
            AssertMappedAccount(page.Results[0]);
        }

        /// <summary>
        /// Maps all FMA statistics independently of the current culture.
        /// </summary>
        [TestMethod]
        public void MapReportResponse_FindFmaAccount_MapsAllStatistics() {
            // Arrange
            const string rawJson = @"{'id':'FMA_contract','payment_processing_statistics':{
                'card_transaction_limit_amount':'10101','card_monthly_limit_amount':'20202',
                'bank_transfer_per_transaction_limit_amount':'30303','bank_transfer_monthly_limit_amount':'40404',
                'bank_transfer_month_to_date_processed_amount':'50505','card_month_to_date_processed_amount':'60606',
                'available_balance_amount':'70707','pending_balance_amount':'80808','reserve_balance_amount':'90909',
                'allowed_negative_balance_amount':'-1010','disable_payout_card_limit_percentage':'300.5',
                'disable_payout_bank_transfer_limit_percentage':'250.5','card_month_to_date_processed_percentage':'12.75',
                'bank_transfer_month_to_date_processed_percentage':'10.25'}}";
            var oldCulture = CultureInfo.CurrentCulture;
            try {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

                // Act
                var account = GpApiMapping.MapReportResponse<MerchantAccountSummary>(rawJson, ReportType.FindAccountDetail);

                // Assert
                Assert.IsNotNull(account);
                Assert.AreEqual("FMA_contract", account.Id);
                var stats = account.PaymentProcessingStatistics;
                Assert.IsNotNull(stats);
                Assert.AreEqual(101.01m, stats.CardTransactionLimitAmount);
                Assert.AreEqual(202.02m, stats.CardMonthlyLimitAmount);
                Assert.AreEqual(303.03m, stats.BankTransferPerTransactionLimitAmount);
                Assert.AreEqual(404.04m, stats.BankTransferMonthlyLimitAmount);
                Assert.AreEqual(505.05m, stats.BankTransferMonthToDateProcessedAmount);
                Assert.AreEqual(606.06m, stats.CardMonthToDateProcessedAmount);
                Assert.AreEqual(707.07m, stats.AvailableBalanceAmount);
                Assert.AreEqual(808.08m, stats.PendingBalanceAmount);
                Assert.AreEqual(909.09m, stats.ReserveBalanceAmount);
                Assert.AreEqual(-10.10m, stats.AllowedNegativeBalanceAmount);
                Assert.AreEqual(300.5m, stats.DisablePayoutCardLimitPercentage);
                Assert.AreEqual(250.5m, stats.DisablePayoutBankTransferLimitPercentage);
                Assert.AreEqual(12.75m, stats.CardMonthToDateProcessedPercentage);
                Assert.AreEqual(10.25m, stats.BankTransferMonthToDateProcessedPercentage);
            }
            finally {
                CultureInfo.CurrentCulture = oldCulture;
            }
        }

        /// <summary>
        /// Preserves absent account sections and legacy configuration strings.
        /// </summary>
        [TestMethod]
        public void MapAccount_WithOptionalFieldsMissing_PreservesLegacyConfigurations() {
            // Arrange
            const string rawJson = @"{'id':'FMA_contract','type':null,'status':null,'configurations':['legacy']}";

            // Act
            var account = GpApiMapping.MapReportResponse<MerchantAccountSummary>(rawJson, ReportType.FindAccountDetail);

            // Assert
            Assert.IsNotNull(account);
            Assert.AreEqual("FMA_contract", account.Id);
            CollectionAssert.AreEqual(new[] { "legacy" }, account.Configurations);
            Assert.IsNull(account.AccountConfigurations);
            Assert.IsNull(account.Channels);
            Assert.IsNull(account.Type);
            Assert.IsNull(account.Status);
            Assert.IsNull(account.Action);
            Assert.IsNull(account.PaymentProcessingStatistics);
            Assert.IsNull(account.Platform);
        }

        /// <summary>
        /// Maps an empty account list without synthesizing action metadata.
        /// </summary>
        [TestMethod]
        public void MapAccounts_WithEmptyResults_ReturnsEmptyPage() {
            // Arrange
            const string rawJson = @"{'total_record_count':0,'current_page_size':0,'accounts':[]}";

            // Act
            var page = GpApiMapping.MapReportResponse<PagedResult<MerchantAccountSummary>>(rawJson, ReportType.FindAccountsPaged);

            // Assert
            Assert.IsNotNull(page);
            Assert.AreEqual(0, page.TotalRecordCount);
            Assert.AreEqual(0, page.CurrentPageSize);
            Assert.IsNotNull(page.Results);
            Assert.AreEqual(0, page.Results.Count);
            Assert.IsNull(page.Action);
        }

        /// <summary>
        /// Distinguishes absent or empty account statistics from explicit zero.
        /// </summary>
        [TestMethod]
        public void MapAccount_WithPartialObjects_ReturnsNullableValues() {
            // Arrange
            const string rawJson = @"{'configurations':[{}],'platform':{},
                'payment_processing_statistics':{'available_balance_amount':'','pending_balance_amount':'0'}}";

            // Act
            var account = GpApiMapping.MapReportResponse<MerchantAccountSummary>(rawJson, ReportType.FindAccountDetail);

            // Assert
            Assert.IsNotNull(account);
            Assert.IsNotNull(account.AccountConfigurations);
            Assert.AreEqual(1, account.AccountConfigurations.Count);
            Assert.IsNull(account.AccountConfigurations[0].Id);
            Assert.IsNull(account.AccountConfigurations[0].Status);
            Assert.IsNull(account.AccountConfigurations[0].SerialNumber);
            Assert.IsNull(account.AccountConfigurations[0].Actions);
            Assert.IsNull(account.AccountConfigurations[0].Platform);
            Assert.IsNull(account.AccountConfigurations[0].Countries);
            Assert.IsNull(account.AccountConfigurations[0].Currencies);
            Assert.IsNull(account.AccountConfigurations[0].Channels);
            Assert.IsNotNull(account.AccountConfigurations[0].PaymentMethods);
            Assert.AreEqual(0, account.AccountConfigurations[0].PaymentMethods.Count);
            Assert.IsNotNull(account.PaymentProcessingStatistics);
            Assert.IsNull(account.PaymentProcessingStatistics.AvailableBalanceAmount);
            Assert.AreEqual(0m, account.PaymentProcessingStatistics.PendingBalanceAmount);
            Assert.IsNull(account.PaymentProcessingStatistics.CardTransactionLimitAmount);
            Assert.IsNotNull(account.Platform);
            Assert.IsNull(account.Platform.Id);
            Assert.IsNull(account.Platform.Reference);
            Assert.IsNull(account.Platform.Name);
            Assert.IsNull(account.Platform.PlatformName);
            Assert.IsNull(account.Platform.PlatformMid);
            Assert.IsNull(account.Platform.MaskedCertstrKeyLast4);
            Assert.IsNull(account.Platform.TermId);
            Assert.IsNull(account.Platform.Country);
            Assert.IsNull(account.Platform.Email);
            Assert.IsNull(account.Platform.Authentication);
            Assert.IsNull(account.Platform.MaskedSecretKeyLast4);
        }

        /// <summary>
        /// Maps every platform field returned by the FMA account endpoint.
        /// </summary>
        [TestMethod]
        public void MapAccount_WithPlatform_MapsAllFields() {
            const string rawJson = @"{'id':'FMA_contract','platform':{
                'id':'PFC_contract','reference':'PTF_contract','name':'PROPAY',
                'platform_name':'Contract platform','platform_mid':'123456789',
                'masked_certstr_key_last4':'XXXXXXXX1234','termid':'123456'}}";

            var account = GpApiMapping.MapReportResponse<MerchantAccountSummary>(rawJson, ReportType.FindAccountDetail);

            Assert.IsNotNull(account);
            Assert.AreEqual("FMA_contract", account.Id);
            Assert.IsNotNull(account.Platform);
            Assert.AreEqual("PFC_contract", account.Platform.Id);
            Assert.AreEqual("PTF_contract", account.Platform.Reference);
            Assert.AreEqual("PROPAY", account.Platform.Name);
            Assert.AreEqual("Contract platform", account.Platform.PlatformName);
            Assert.AreEqual("123456789", account.Platform.PlatformMid);
            Assert.AreEqual("XXXXXXXX1234", account.Platform.MaskedCertstrKeyLast4);
            Assert.AreEqual("123456", account.Platform.TermId);
        }

        /// <summary>
        /// Rejects malformed account statistics rather than silently converting them to zero.
        /// </summary>
        [TestMethod]
        public void MapFmaAccount_WithInvalidAmount_ThrowsGatewayException() {
            // Arrange
            const string rawJson = @"{'payment_processing_statistics':{'available_balance_amount':'invalid'}}";

            // Act
            var error = Assert.ThrowsException<GatewayException>(() =>
                GpApiMapping.MapReportResponse<MerchantAccountSummary>(rawJson, ReportType.FindAccountDetail));

            // Assert
            Assert.AreEqual("Invalid GP-API account statistic: available_balance_amount", error.Message);
        }

        /// <summary>
        /// Asserts every common account field and nested configuration field.
        /// </summary>
        /// <param name="account">The mapped account.</param>
        private static void AssertMappedAccount(MerchantAccountSummary account) {
            Assert.IsNotNull(account);
            Assert.AreEqual("FMA_contract", account.Id);
            Assert.AreEqual("Fund management", account.Name);
            Assert.AreEqual(MerchantAccountType.FUND_MANAGEMENT, account.Type);
            Assert.AreEqual(MerchantAccountStatus.ACTIVE, account.Status);
            CollectionAssert.AreEqual(new[] { "ACC_GET" }, account.Permissions);
            CollectionAssert.AreEqual(new[] { "US" }, account.Countries);
            CollectionAssert.AreEqual(new[] { "USD" }, account.Currencies);
            CollectionAssert.AreEqual(new[] { Channel.CardNotPresent }, account.Channels);
            CollectionAssert.AreEqual(new[] { PaymentMethodName.Card, PaymentMethodName.BankTransfer }, account.PaymentMethods);
            Assert.AreEqual("accounts@example.com", account.EmailId);
            Assert.AreEqual("MER_contract", account.MerchantId);
            Assert.AreEqual("Contract merchant", account.MerchantName);
            Assert.AreEqual("2026-01-02T03:04:05.678Z", account.TimeCreated);
            Assert.AreEqual("2026-01-02T03:04:05.678Z", account.TimeLastUpdated);
            Assert.AreEqual("LOST", account.CardReplacementReason);
            CollectionAssert.AreEqual(new[] { "PAY_IN_ENABLE", "PAY_OUT_DISABLE" }, account.Capabilities);
            Assert.IsNotNull(account.AccountConfigurations);
            Assert.AreEqual(1, account.AccountConfigurations.Count);
            var configuration = account.AccountConfigurations[0];
            Assert.IsNotNull(configuration);
            Assert.AreEqual("RTC_contract", configuration.Id);
            Assert.AreEqual("ACTIVE", configuration.Status);
            Assert.AreEqual("serial_contract", configuration.SerialNumber);
            CollectionAssert.AreEqual(new[] { "AUTHORIZE", "LINK_LIST" }, configuration.Actions);
            Assert.IsNotNull(configuration.Platform);
            Assert.AreEqual("PFC_configuration", configuration.Platform.Id);
            Assert.AreEqual("PTF_configuration", configuration.Platform.Reference);
            Assert.AreEqual("TRUUST", configuration.Platform.Name);
            Assert.AreEqual("Configuration platform", configuration.Platform.PlatformName);
            Assert.AreEqual("configuration_mid", configuration.Platform.PlatformMid);
            Assert.AreEqual("GB", configuration.Platform.Country);
            Assert.AreEqual("platform@example.com", configuration.Platform.Email);
            Assert.AreEqual("ENABLE", configuration.Platform.Authentication);
            Assert.AreEqual("XXXXXXXX1234", configuration.Platform.MaskedSecretKeyLast4);
            Assert.IsNull(configuration.Platform.MaskedCertstrKeyLast4);
            Assert.IsNull(configuration.Platform.TermId);
            CollectionAssert.AreEqual(new[] { "US" }, configuration.Countries);
            CollectionAssert.AreEqual(new[] { "USD" }, configuration.Currencies);
            CollectionAssert.AreEqual(new[] { Channel.CardNotPresent }, configuration.Channels);
            CollectionAssert.AreEqual(new[] { PaymentMethodName.Card, PaymentMethodName.BankTransfer }, configuration.PaymentMethods);
            Assert.IsNotNull(account.Addresses);
            Assert.AreEqual(1, account.Addresses.Count);
            Assert.AreEqual("Line 1", account.Addresses[0].StreetAddress1);
            Assert.AreEqual("Line 2", account.Addresses[0].StreetAddress2);
            Assert.AreEqual("Line 3", account.Addresses[0].StreetAddress3);
            Assert.AreEqual("Atlanta", account.Addresses[0].City);
            Assert.AreEqual("GA", account.Addresses[0].State);
            Assert.AreEqual("30346", account.Addresses[0].PostalCode);
            Assert.AreEqual("US", account.Addresses[0].CountryCode);
            AssertMappedAccountAction(account.Action, "ACCOUNTS_SINGLE");
        }

        /// <summary>
        /// Asserts all account action metadata fields.
        /// </summary>
        /// <param name="action">The mapped action.</param>
        /// <param name="type">The expected account operation.</param>
        private static void AssertMappedAccountAction(Entities.Action action, string type) {
            Assert.IsNotNull(action);
            Assert.AreEqual("ACT_contract", action.Id);
            Assert.AreEqual(type, action.Type);
            Assert.AreEqual("2026-01-02T03:04:05.678Z", action.TimeCreated);
            Assert.AreEqual("SUCCESS", action.ResultCode);
            Assert.AreEqual("APP_contract", action.AppId);
            Assert.AreEqual("Contract app", action.AppName);
        }

        [TestMethod]
        public void MapTransactionSummaryTest() {
            // Arrange
            string rawJson = "{\"id\":\"TRN_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz\",\"time_created\":\"2020-06-05T03:08:20.896Z\",\"time_last_updated\":\"\",\"status\":\"PREAUTHORIZED\",\"type\":\"SALE\",\"merchant_id\":\"MER_c4c0df11039c48a9b63701adeaa296c3\",\"merchant_name\":\"Sandbox_merchant_2\",\"account_id\":\"TRA_6716058969854a48b33347043ff8225f\",\"account_name\":\"Transaction_Processing\",\"channel\":\"CNP\",\"amount\":\"10000\",\"currency\":\"CAD\",\"reference\":\"My-TRANS-184398775\",\"description\":\"41e7877b-da90-4c5f-befe-7f024b96311e\",\"order_reference\":\"\",\"time_created_reference\":\"\",\"batch_id\":\"\",\"initiator\":\"\",\"country\":\"\",\"language\":\"\",\"ip_address\":\"97.107.232.5\",\"site_reference\":\"\",\"payment_method\":{\"result\":\"00\",\"message\":\"SUCCESS\",\"entry_mode\":\"ECOM\",\"name\":\"NAME NOT PROVIDED\",\"card\":{\"funding\":\"CREDIT\",\"brand\":\"VISA\",\"authcode\":\"12345\",\"brand_reference\":\"TQ76bJf7qzkC30U0\",\"masked_number_first6last4\":\"411111XXXXXX1111\",\"cvv_indicator\":\"PRESENT\",\"cvv_result\":\"MATCHED\",\"avs_address_result\":\"MATCHED\",\"avs_postal_code_result\":\"MATCHED\"}},\"action_create_id\":\"ACT_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz\",\"parent_resource_id\":\"TRN_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz\",\"action\":{\"id\":\"ACT_kLkU0qND7wyuW0Br76ZNyAnlPTjHsb\",\"type\":\"TRANSACTION_SINGLE\",\"time_created\":\"2020-11-24T15:43:43.990Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0\",\"app_name\":\"test_app\"}}";
            
            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            TransactionSummary transaction = GpApiMapping.MapTransactionSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), transaction.TransactionId);
            Assert.AreEqual(doc.GetValue<DateTime>("time_created"), transaction.TransactionDate);
            Assert.AreEqual(doc.GetValue<string>("status"), transaction.TransactionStatus);
            Assert.AreEqual(doc.GetValue<string>("type"), transaction.TransactionType);
            Assert.AreEqual(doc.GetValue<string>("channel"), transaction.Channel);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), transaction.Amount);
            Assert.AreEqual(doc.GetValue<string>("currency"), transaction.Currency);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ReferenceNumber);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ClientTransactionId);
            Assert.AreEqual(doc.GetValue<string>("batch_id"), transaction.BatchId);
            Assert.AreEqual(doc.GetValue<string>("batch_id"), transaction.BatchSequenceNumber);
            Assert.AreEqual(doc.GetValue<string>("country"), transaction.Country);
            Assert.AreEqual(doc.GetValue<string>("parent_resource_id"), transaction.OriginalTransactionId);
            
            var paymentMethod = doc.Get("payment_method");
            Assert.AreEqual(paymentMethod?.GetValue<string>("message"), transaction.GatewayResponseMessage);
            Assert.AreEqual(paymentMethod?.GetValue<string>("entry_mode"), transaction.EntryMode);
            Assert.AreEqual(paymentMethod?.GetValue<string>("name"), transaction.CardHolderName);

            var card = paymentMethod.Get("card");
            Assert.AreEqual(card?.GetValue<string>("brand"), transaction.CardType);
            Assert.AreEqual(card?.GetValue<string>("authcode"), transaction.AuthCode);
            Assert.AreEqual(card?.GetValue<string>("brand_reference"), transaction.BrandReference);
            Assert.AreEqual(card?.GetValue<string>("arn"), transaction.AquirerReferenceNumber);
            Assert.AreEqual(card?.GetValue<string>("masked_number_first6last4"), transaction.MaskedCardNumber);
        }

        [TestMethod]
        public void MapTransactionSummaryTest_NullDates()
        {
            // Arrange
            string rawJson = "{\"id\":\"TRN_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz\",\"time_created\":\"\",\"time_last_updated\":\"\",\"status\":\"PREAUTHORIZED\",\"type\":\"SALE\",\"merchant_id\":\"MER_c4c0df11039c48a9b63701adeaa296c3\",\"merchant_name\":\"Sandbox_merchant_2\",\"account_id\":\"TRA_6716058969854a48b33347043ff8225f\",\"account_name\":\"Transaction_Processing\",\"channel\":\"CNP\",\"amount\":\"10000\",\"currency\":\"CAD\",\"reference\":\"My-TRANS-184398775\",\"description\":\"41e7877b-da90-4c5f-befe-7f024b96311e\",\"order_reference\":\"\",\"time_created_reference\":\"\",\"batch_id\":\"\",\"initiator\":\"\",\"country\":\"\",\"language\":\"\",\"ip_address\":\"97.107.232.5\",\"site_reference\":\"\",\"payment_method\":{\"result\":\"00\",\"message\":\"SUCCESS\",\"entry_mode\":\"ECOM\",\"name\":\"NAME NOT PROVIDED\",\"card\":{\"funding\":\"CREDIT\",\"brand\":\"VISA\",\"authcode\":\"12345\",\"brand_reference\":\"TQ76bJf7qzkC30U0\",\"masked_number_first6last4\":\"411111XXXXXX1111\",\"cvv_indicator\":\"PRESENT\",\"cvv_result\":\"MATCHED\",\"avs_address_result\":\"MATCHED\",\"avs_postal_code_result\":\"MATCHED\"}},\"action_create_id\":\"ACT_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz\",\"parent_resource_id\":\"TRN_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz\",\"action\":{\"id\":\"ACT_kLkU0qND7wyuW0Br76ZNyAnlPTjHsb\",\"type\":\"TRANSACTION_SINGLE\",\"time_created\":\"\",\"result_code\":\"SUCCESS\",\"app_id\":\"JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0\",\"app_name\":\"test_app\"}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            TransactionSummary transaction = GpApiMapping.MapTransactionSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), transaction.TransactionId);
            Assert.IsNull(transaction.TransactionDate);
            Assert.AreEqual(doc.GetValue<string>("status"), transaction.TransactionStatus);
            Assert.AreEqual(doc.GetValue<string>("type"), transaction.TransactionType);
            Assert.AreEqual(doc.GetValue<string>("channel"), transaction.Channel);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), transaction.Amount);
            Assert.AreEqual(doc.GetValue<string>("currency"), transaction.Currency);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ReferenceNumber);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ClientTransactionId);
            Assert.AreEqual(doc.GetValue<string>("batch_id"), transaction.BatchId);
            Assert.AreEqual(doc.GetValue<string>("batch_id"), transaction.BatchSequenceNumber);
            Assert.AreEqual(doc.GetValue<string>("country"), transaction.Country);
            Assert.AreEqual(doc.GetValue<string>("parent_resource_id"), transaction.OriginalTransactionId);

            var paymentMethod = doc.Get("payment_method");
            Assert.AreEqual(paymentMethod?.GetValue<string>("message"), transaction.GatewayResponseMessage);
            Assert.AreEqual(paymentMethod?.GetValue<string>("entry_mode"), transaction.EntryMode);
            Assert.AreEqual(paymentMethod?.GetValue<string>("name"), transaction.CardHolderName);

            var card = paymentMethod.Get("card");
            Assert.AreEqual(card?.GetValue<string>("brand"), transaction.CardType);
            Assert.AreEqual(card?.GetValue<string>("authcode"), transaction.AuthCode);
            Assert.AreEqual(card?.GetValue<string>("brand_reference"), transaction.BrandReference);
            Assert.AreEqual(card?.GetValue<string>("arn"), transaction.AquirerReferenceNumber);
            Assert.AreEqual(card?.GetValue<string>("masked_number_first6last4"), transaction.MaskedCardNumber);
        }

        [TestMethod]
        public void MapTransactionSummaryTest_FromObject() {
            // Arrange
            dynamic obj = new ExpandoObject();
            obj.id = "TRN_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz";
            obj.time_created = DateTime.UtcNow;
            obj.status = "PREAUTHORIZED";
            obj.type = "SALE";
            obj.channel = "CNP";
            obj.amount = "10000";
            obj.currency = "USD";
            obj.reference = "My-TRANS-184398775";
            obj.batch_id = "BATCH_123456";
            obj.country = "US";
            obj.parent_resource_id = "PARENT_456123";
            obj.payment_method = new ExpandoObject();
            obj.payment_method.message = "SUCCESS";
            obj.payment_method.entry_mode = "ECOM";
            obj.payment_method.name = "James Mason";
            obj.payment_method.card = new ExpandoObject();
            obj.payment_method.card.brand = "VISA";
            obj.payment_method.card.authcode = "12345";
            obj.payment_method.card.brand_reference = "TQ76bJf7qzkC30U0";
            obj.payment_method.card.arn = "ARN_123456798";
            obj.payment_method.card.masked_number_first6last4 = "411111XXXXXX1111";

            string rawJson = JsonConvert.SerializeObject(obj); 

            // Act
            TransactionSummary transaction = GpApiMapping.MapTransactionSummary(JsonDoc.Parse(rawJson));

            // Assert
            Assert.AreEqual(obj.id, transaction.TransactionId);
            Assert.AreEqual(obj.time_created, transaction.TransactionDate);
            Assert.AreEqual(obj.status, transaction.TransactionStatus);
            Assert.AreEqual(obj.type, transaction.TransactionType);
            Assert.AreEqual(obj.channel, transaction.Channel);
            Assert.AreEqual((obj.amount as string).ToAmount(), transaction.Amount);
            Assert.AreEqual(obj.currency, transaction.Currency);
            Assert.AreEqual(obj.reference, transaction.ReferenceNumber);
            Assert.AreEqual(obj.reference, transaction.ClientTransactionId);
            Assert.AreEqual(obj.batch_id, transaction.BatchId);
            Assert.AreEqual(obj.batch_id, transaction.BatchSequenceNumber);
            Assert.AreEqual(obj.country, transaction.Country);
            Assert.AreEqual(obj.parent_resource_id, transaction.OriginalTransactionId);
            Assert.AreEqual(obj.payment_method.message, transaction.GatewayResponseMessage);
            Assert.AreEqual(obj.payment_method.entry_mode, transaction.EntryMode);
            Assert.AreEqual(obj.payment_method.name, transaction.CardHolderName);
            Assert.AreEqual(obj.payment_method.card.brand, transaction.CardType);
            Assert.AreEqual(obj.payment_method.card.authcode, transaction.AuthCode);
            Assert.AreEqual(obj.payment_method.card.brand_reference, transaction.BrandReference);
            Assert.AreEqual(obj.payment_method.card.arn, transaction.AquirerReferenceNumber);
            Assert.AreEqual(obj.payment_method.card.masked_number_first6last4, transaction.MaskedCardNumber);
        }

        [TestMethod]
        public void MapTransactionSummaryTest_FromObject_NullDates()
        {
            // Arrange
            dynamic obj = new ExpandoObject();
            obj.id = "TRN_TvY1QFXxQKtaFSjNaLnDVdo3PZ7ivz";
            obj.time_created = null;
            obj.status = "PREAUTHORIZED";
            obj.type = "SALE";
            obj.channel = "CNP";
            obj.amount = "10000";
            obj.currency = "USD";
            obj.reference = "My-TRANS-184398775";
            obj.batch_id = "BATCH_123456";
            obj.country = "US";
            obj.parent_resource_id = "PARENT_456123";
            obj.payment_method = new ExpandoObject();
            obj.payment_method.message = "SUCCESS";
            obj.payment_method.entry_mode = "ECOM";
            obj.payment_method.name = "James Mason";
            obj.payment_method.card = new ExpandoObject();
            obj.payment_method.card.brand = "VISA";
            obj.payment_method.card.authcode = "12345";
            obj.payment_method.card.brand_reference = "TQ76bJf7qzkC30U0";
            obj.payment_method.card.arn = "ARN_123456798";
            obj.payment_method.card.masked_number_first6last4 = "411111XXXXXX1111";

            string rawJson = JsonConvert.SerializeObject(obj);

            // Act
            TransactionSummary transaction = GpApiMapping.MapTransactionSummary(JsonDoc.Parse(rawJson));

            // Assert
            Assert.AreEqual(obj.id, transaction.TransactionId);
            Assert.AreEqual(obj.time_created, transaction.TransactionDate);
            Assert.AreEqual(obj.status, transaction.TransactionStatus);
            Assert.AreEqual(obj.type, transaction.TransactionType);
            Assert.AreEqual(obj.channel, transaction.Channel);
            Assert.AreEqual((obj.amount as string).ToAmount(), transaction.Amount);
            Assert.AreEqual(obj.currency, transaction.Currency);
            Assert.AreEqual(obj.reference, transaction.ReferenceNumber);
            Assert.AreEqual(obj.reference, transaction.ClientTransactionId);
            Assert.AreEqual(obj.batch_id, transaction.BatchId);
            Assert.AreEqual(obj.batch_id, transaction.BatchSequenceNumber);
            Assert.AreEqual(obj.country, transaction.Country);
            Assert.AreEqual(obj.parent_resource_id, transaction.OriginalTransactionId);
            Assert.AreEqual(obj.payment_method.message, transaction.GatewayResponseMessage);
            Assert.AreEqual(obj.payment_method.entry_mode, transaction.EntryMode);
            Assert.AreEqual(obj.payment_method.name, transaction.CardHolderName);
            Assert.AreEqual(obj.payment_method.card.brand, transaction.CardType);
            Assert.AreEqual(obj.payment_method.card.authcode, transaction.AuthCode);
            Assert.AreEqual(obj.payment_method.card.brand_reference, transaction.BrandReference);
            Assert.AreEqual(obj.payment_method.card.arn, transaction.AquirerReferenceNumber);
            Assert.AreEqual(obj.payment_method.card.masked_number_first6last4, transaction.MaskedCardNumber);
        }

        [TestMethod]
        public void MapDepositSummaryTest() {
            // Arrange
            string rawJson = "{\"id\":\"DEP_2342423423\",\"time_created\":\"2020-11-21\",\"status\":\"FUNDED\",\"funding_type\":\"CREDIT\",\"amount\":\"11400\",\"currency\":\"USD\",\"aggregation_model\":\"H-By Date\",\"bank_transfer\":{\"masked_account_number_last4\":\"XXXXXX9999\",\"bank\":{\"code\":\"XXXXX0001\"}},\"system\":{\"mid\":\"101023947262\",\"hierarchy\":\"055-70-024-011-019\",\"name\":\"XYZ LTD.\",\"dba\":\"XYZ Group\"},\"sales\":{\"count\":4,\"amount\":\"12400\"},\"refunds\":{\"count\":1,\"amount\":\"-1000\"},\"discounts\":{\"count\":0,\"amount\":\"\"},\"tax\":{\"count\":0,\"amount\":\"\"},\"disputes\":{\"chargebacks\":{\"count\":0,\"amount\":\"\"},\"reversals\":{\"count\":0,\"amount\":\"\"}},\"fees\":{\"amount\":\"\"},\"action\":{\"id\":\"ACT_TWdmMMOBZ91iQX1DcvxYermuVJ6E6h\",\"type\":\"DEPOSIT_SINGLE\",\"time_created\":\"2020-11-24T18:43:43.370Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0\",\"app_name\":\"test_app\"}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            DepositSummary deposit = GpApiMapping.MapDepositSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), deposit.DepositId);
            Assert.AreEqual(doc.GetValue<DateTime>("time_created"), deposit.DepositDate);
            Assert.AreEqual(doc.GetValue<string>("status"), deposit.Status);
            Assert.AreEqual(doc.GetValue<string>("funding_type"), deposit.Type);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), deposit.Amount);
            Assert.AreEqual(doc.GetValue<string>("currency"), deposit.Currency);

            Assert.AreEqual(doc.Get("system")?.GetValue<string>("mid"), deposit.MerchantNumber);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("hierarchy"), deposit.MerchantHierarchy);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("name"), deposit.MerchantName);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("dba"), deposit.MerchantDbaName);

            Assert.AreEqual(doc.Get("sales")?.GetValue<int>("count") ?? default(int), deposit.SalesTotalCount);
            Assert.AreEqual(doc.Get("sales")?.GetValue<string>("amount").ToAmount(), deposit.SalesTotalAmount);

            Assert.AreEqual(doc.Get("refunds")?.GetValue<int>("count") ?? default(int), deposit.RefundsTotalCount);
            Assert.AreEqual(doc.Get("refunds")?.GetValue<string>("amount").ToAmount(), deposit.RefundsTotalAmount);

            Assert.AreEqual(doc.Get("disputes")?.Get("chargebacks")?.GetValue<int>("count") ?? default(int), deposit.ChargebackTotalCount);
            Assert.AreEqual(doc.Get("disputes")?.Get("chargebacks")?.GetValue<string>("amount").ToAmount(), deposit.ChargebackTotalAmount);

            Assert.AreEqual(doc.Get("disputes")?.Get("reversals")?.GetValue<int>("count") ?? default(int), deposit.AdjustmentTotalCount);
            Assert.AreEqual(doc.Get("disputes")?.Get("reversals")?.GetValue<string>("amount").ToAmount(), deposit.AdjustmentTotalAmount);

            Assert.AreEqual(doc.Get("fees")?.GetValue<string>("amount").ToAmount(), deposit.FeesTotalAmount);
        }

        [TestMethod]
        public void MapDepositSummaryTest_NullDates()
        {
            // Arrange
            string rawJson = "{\"id\":\"DEP_2342423423\",\"time_created\":\"\",\"status\":\"FUNDED\",\"funding_type\":\"CREDIT\",\"amount\":\"11400\",\"currency\":\"USD\",\"aggregation_model\":\"H-By Date\",\"bank_transfer\":{\"masked_account_number_last4\":\"XXXXXX9999\",\"bank\":{\"code\":\"XXXXX0001\"}},\"system\":{\"mid\":\"101023947262\",\"hierarchy\":\"055-70-024-011-019\",\"name\":\"XYZ LTD.\",\"dba\":\"XYZ Group\"},\"sales\":{\"count\":4,\"amount\":\"12400\"},\"refunds\":{\"count\":1,\"amount\":\"-1000\"},\"discounts\":{\"count\":0,\"amount\":\"\"},\"tax\":{\"count\":0,\"amount\":\"\"},\"disputes\":{\"chargebacks\":{\"count\":0,\"amount\":\"\"},\"reversals\":{\"count\":0,\"amount\":\"\"}},\"fees\":{\"amount\":\"\"},\"action\":{\"id\":\"ACT_TWdmMMOBZ91iQX1DcvxYermuVJ6E6h\",\"type\":\"DEPOSIT_SINGLE\",\"time_created\":\"\",\"result_code\":\"SUCCESS\",\"app_id\":\"JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0\",\"app_name\":\"test_app\"}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            DepositSummary deposit = GpApiMapping.MapDepositSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), deposit.DepositId);
            Assert.IsNull(deposit.DepositDate);
            Assert.AreEqual(doc.GetValue<string>("status"), deposit.Status);
            Assert.AreEqual(doc.GetValue<string>("funding_type"), deposit.Type);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), deposit.Amount);
            Assert.AreEqual(doc.GetValue<string>("currency"), deposit.Currency);

            Assert.AreEqual(doc.Get("system")?.GetValue<string>("mid"), deposit.MerchantNumber);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("hierarchy"), deposit.MerchantHierarchy);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("name"), deposit.MerchantName);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("dba"), deposit.MerchantDbaName);

            Assert.AreEqual(doc.Get("sales")?.GetValue<int>("count") ?? default(int), deposit.SalesTotalCount);
            Assert.AreEqual(doc.Get("sales")?.GetValue<string>("amount").ToAmount(), deposit.SalesTotalAmount);

            Assert.AreEqual(doc.Get("refunds")?.GetValue<int>("count") ?? default(int), deposit.RefundsTotalCount);
            Assert.AreEqual(doc.Get("refunds")?.GetValue<string>("amount").ToAmount(), deposit.RefundsTotalAmount);

            Assert.AreEqual(doc.Get("disputes")?.Get("chargebacks")?.GetValue<int>("count") ?? default(int), deposit.ChargebackTotalCount);
            Assert.AreEqual(doc.Get("disputes")?.Get("chargebacks")?.GetValue<string>("amount").ToAmount(), deposit.ChargebackTotalAmount);

            Assert.AreEqual(doc.Get("disputes")?.Get("reversals")?.GetValue<int>("count") ?? default(int), deposit.AdjustmentTotalCount);
            Assert.AreEqual(doc.Get("disputes")?.Get("reversals")?.GetValue<string>("amount").ToAmount(), deposit.AdjustmentTotalAmount);

            Assert.AreEqual(doc.Get("fees")?.GetValue<string>("amount").ToAmount(), deposit.FeesTotalAmount);
        }

        [TestMethod]
        public void MapDisputeSummaryTest() {
            // Arrange
            string rawJson = "{\"id\":\"DIS_SAND_abcd1234\",\"time_created\":\"2020-11-12T18:50:39.721Z\",\"merchant_id\":\"MER_62251730c5574bbcb268191b5f315de8\",\"merchant_name\":\"TEST MERCHANT\",\"account_id\":\"DIA_882c832d13e04185bb6e213d6303ed98\",\"account_name\":\"testdispute\",\"status\":\"WITH_MERCHANT\",\"status_time_created\":\"2020-11-14T18:50:39.721Z\",\"stage\":\"RETRIEVAL\",\"stage_time_created\":\"2020-11-17T18:50:39.722Z\",\"amount\":\"1000\",\"currency\":\"USD\",\"payer_amount\":\"1000\",\"payer_currency\":\"USD\",\"merchant_amount\":\"1000\",\"merchant_currency\":\"USD\",\"reason_code\":\"104\",\"reason_description\":\"Other Fraud-Card Absent Environment\",\"time_to_respond_by\":\"2020-11-29T18:50:39.722Z\",\"result\":\"PENDING\",\"investigator_comment\":\"WITH_MERCHANT RETRIEVAL PENDING 1000 USD 1000 USD\",\"system\":{\"mid\":\"11112334\",\"tid\":\"22229876\",\"hierarchy\":\"000-00-000-000-000\",\"name\":\"MERCHANT ABC INC.\",\"dba\":\"MERCHANT XYZ INC.\"},\"last_adjustment_amount\":\"\",\"last_adjustment_currency\":\"\",\"last_adjustment_funding\":\"\",\"last_adjustment_time_created\":\"2020-11-20T18:50:39.722Z\",\"net_financial_amount\":\"\",\"net_financial_currency\":\"\",\"net_financial_funding\":\"\",\"payment_method_provider\":[{\"comment\":\"issuer comments 34523\",\"reference\":\"issuer-reference-0001\",\"documents\":[{\"id\":\"DOC_MyEvidence_234234AVCDE-1\"}]}],\"transaction\":{\"time_created\":\"2020-10-05T18:50:39.726Z\",\"type\":\"SALE\",\"amount\":\"1000\",\"currency\":\"USD\",\"reference\":\"my-trans-AAA1\",\"remarks\":\"my-trans-AAA1\",\"payment_method\":{\"card\":{\"number\":\"424242xxxxxx4242\",\"arn\":\"834523482349123\",\"brand\":\"VISA\",\"authcode\":\"234AB\",\"brand_reference\":\"23423421342323A\"}}},\"documents\":[],\"action\":{\"id\":\"ACT_5blBTHnIs4aOCIvGwG7KizYUpsGI0g\",\"type\":\"DISPUTE_SINGLE\",\"time_created\":\"2020-11-24T18:50:39.925Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0\",\"app_name\":\"test_app\"}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            DisputeSummary dispute = GpApiMapping.MapDisputeSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), dispute.CaseId);
            Assert.AreEqual(doc.GetValue<DateTime>("time_created"), dispute.CaseIdTime);
            Assert.AreEqual(doc.GetValue<string>("status"), dispute.CaseStatus);
            Assert.AreEqual(doc.GetValue<string>("stage"), dispute.CaseStage);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), dispute.CaseAmount);
            Assert.AreEqual(doc.GetValue<string>("currency"), dispute.CaseCurrency);
            Assert.AreEqual(doc.GetValue<string>("reason_code"), dispute.ReasonCode);
            Assert.AreEqual(doc.GetValue<string>("reason_description"), dispute.Reason);
            Assert.AreEqual(doc.GetValue<DateTime>("time_to_respond_by"), dispute.RespondByDate);
            Assert.AreEqual(doc.GetValue<string>("result"), dispute.Result);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("mid"), dispute.CaseMerchantId);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("tid"), dispute.CaseTerminalId);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("hierarchy"), dispute.MerchantHierarchy);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("name"), dispute.MerchantName);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("dba"), dispute.MerchantDbaName);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_amount").ToAmount(), dispute.LastAdjustmentAmount);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_currency"), dispute.LastAdjustmentCurrency);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_funding"), dispute.LastAdjustmentFunding);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("number"), dispute.TransactionMaskedCardNumber);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("arn"), dispute.TransactionARN);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("brand"), dispute.TransactionCardType);
        }

        [TestMethod]
        public void MapDisputeSummaryTest_NullDate()
        {
            // Arrange
            string rawJson = "{\"id\":\"DIS_SAND_abcd1234\",\"time_created\":\"\",\"merchant_id\":\"MER_62251730c5574bbcb268191b5f315de8\",\"merchant_name\":\"TEST MERCHANT\",\"account_id\":\"DIA_882c832d13e04185bb6e213d6303ed98\",\"account_name\":\"testdispute\",\"status\":\"WITH_MERCHANT\",\"status_time_created\":\"\",\"stage\":\"RETRIEVAL\",\"stage_time_created\":\"\",\"amount\":\"1000\",\"currency\":\"USD\",\"payer_amount\":\"1000\",\"payer_currency\":\"USD\",\"merchant_amount\":\"1000\",\"merchant_currency\":\"USD\",\"reason_code\":\"104\",\"reason_description\":\"Other Fraud-Card Absent Environment\",\"time_to_respond_by\":\"\",\"result\":\"PENDING\",\"investigator_comment\":\"WITH_MERCHANT RETRIEVAL PENDING 1000 USD 1000 USD\",\"system\":{\"mid\":\"11112334\",\"tid\":\"22229876\",\"hierarchy\":\"000-00-000-000-000\",\"name\":\"MERCHANT ABC INC.\",\"dba\":\"MERCHANT XYZ INC.\"},\"last_adjustment_amount\":\"\",\"last_adjustment_currency\":\"\",\"last_adjustment_funding\":\"\",\"last_adjustment_time_created\":\"\",\"net_financial_amount\":\"\",\"net_financial_currency\":\"\",\"net_financial_funding\":\"\",\"payment_method_provider\":[{\"comment\":\"issuer comments 34523\",\"reference\":\"issuer-reference-0001\",\"documents\":[{\"id\":\"DOC_MyEvidence_234234AVCDE-1\"}]}],\"transaction\":{\"time_created\":\"\",\"type\":\"SALE\",\"amount\":\"1000\",\"currency\":\"USD\",\"reference\":\"my-trans-AAA1\",\"remarks\":\"my-trans-AAA1\",\"payment_method\":{\"card\":{\"number\":\"424242xxxxxx4242\",\"arn\":\"834523482349123\",\"brand\":\"VISA\",\"authcode\":\"234AB\",\"brand_reference\":\"23423421342323A\"}}},\"documents\":[],\"action\":{\"id\":\"ACT_5blBTHnIs4aOCIvGwG7KizYUpsGI0g\",\"type\":\"DISPUTE_SINGLE\",\"time_created\":\"\",\"result_code\":\"SUCCESS\",\"app_id\":\"JF2GQpeCrOivkBGsTRiqkpkdKp67Gxi0\",\"app_name\":\"test_app\"}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            DisputeSummary dispute = GpApiMapping.MapDisputeSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), dispute.CaseId);
            Assert.IsNull(dispute.CaseIdTime);
            Assert.AreEqual(doc.GetValue<string>("status"), dispute.CaseStatus);
            Assert.AreEqual(doc.GetValue<string>("stage"), dispute.CaseStage);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), dispute.CaseAmount);
            Assert.AreEqual(doc.GetValue<string>("currency"), dispute.CaseCurrency);
            Assert.AreEqual(doc.GetValue<string>("reason_code"), dispute.ReasonCode);
            Assert.AreEqual(doc.GetValue<string>("reason_description"), dispute.Reason);
            Assert.IsNull(dispute.RespondByDate);
            Assert.AreEqual(doc.GetValue<string>("result"), dispute.Result);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("mid"), dispute.CaseMerchantId);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("tid"), dispute.CaseTerminalId);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("hierarchy"), dispute.MerchantHierarchy);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("name"), dispute.MerchantName);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("dba"), dispute.MerchantDbaName);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_amount").ToAmount(), dispute.LastAdjustmentAmount);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_currency"), dispute.LastAdjustmentCurrency);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_funding"), dispute.LastAdjustmentFunding);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("number"), dispute.TransactionMaskedCardNumber);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("arn"), dispute.TransactionARN);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("brand"), dispute.TransactionCardType);
        }

        [TestMethod]
        public void MapSettlementDisputeSummaryTest() {
            string rawJson = "{\"id\":\"DIS_812\",\"status\":\"FUNDED\",\"stage\":\"CHARGEBACK\",\"stage_time_created\":\"2021-03-16T14:03:44\",\"amount\":\"200\",\"currency\":\"GBP\",\"reason_code\":\"PM\",\"reason_description\":\"Paid by Other Means\",\"time_to_respond_by\":\"2021-04-02T14:03:44\",\"result\":\"LOST\",\"funding_type\":\"DEBIT\",\"deposit_time_created\":\"2021-03-20\",\"deposit_id\":\"DEP_2342423443\",\"last_adjustment_amount\":\"\",\"last_adjustment_currency\":\"\",\"last_adjustment_funding\":\"\",\"last_adjustment_time_created\":\"\",\"system\":{\"mid\":\"11112334\",\"tid\":\"22229876\",\"hierarchy\":\"000-00-000-000-000\",\"name\":\"MERCHANT ABC INC.\",\"dba\":\"MERCHANT XYZ INC.\"},\"transaction\":{\"time_created\":\"2021-02-21T14:03:44\",\"merchant_time_created\":\"2021-02-21T16:03:44\",\"type\":\"SALE\",\"amount\":\"200\",\"currency\":\"GBP\",\"reference\":\"28012076eb6M\",\"payment_method\":{\"card\":{\"masked_number_first6last4\":\"379132XXXXX1007\",\"arn\":\"71400011203688701393903\",\"brand\":\"AMEX\",\"authcode\":\"129623\",\"brand_reference\":\"MWE1P0JG80110\"}}}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            DisputeSummary dispute = GpApiMapping.MapSettlementDisputeSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), dispute.CaseId);
            Assert.AreEqual(doc.GetValue<DateTime>("stage_time_created"), dispute.CaseIdTime);
            Assert.AreEqual(doc.GetValue<string>("status"), dispute.CaseStatus);
            Assert.AreEqual(doc.GetValue<string>("stage"), dispute.CaseStage);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), dispute.CaseAmount);
            Assert.AreEqual(doc.GetValue<string>("currency"), dispute.CaseCurrency);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_amount").ToAmount(), dispute.LastAdjustmentAmount);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_currency"), dispute.LastAdjustmentCurrency);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_funding"), dispute.LastAdjustmentFunding);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("mid"), dispute.CaseMerchantId);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("tid"), dispute.CaseTerminalId);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("hierarchy"), dispute.MerchantHierarchy);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("name"), dispute.MerchantName);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("dba"), dispute.MerchantDbaName);
            Assert.AreEqual(doc.GetValue<string>("reason_code"), dispute.ReasonCode);
            Assert.AreEqual(doc.GetValue<string>("reason_description"), dispute.Reason);
            Assert.AreEqual(doc.GetValue<string>("result"), dispute.Result);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<DateTime?>("time_created"), dispute.TransactionTime);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<string>("type"), dispute.TransactionType);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<string>("amount").ToAmount(), dispute.TransactionAmount);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<string>("currency"), dispute.TransactionCurrency);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<string>("reference"), dispute.TransactionReferenceNumber);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("masked_number_first6last4"), dispute.TransactionMaskedCardNumber);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("arn"), dispute.TransactionARN);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("brand"), dispute.TransactionCardType);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("authcode"), dispute.TransactionAuthCode);
            Assert.AreEqual(doc.GetValue<DateTime>("time_to_respond_by"), dispute.RespondByDate);
            Assert.AreEqual(doc.GetValue<DateTime>("deposit_time_created"), dispute.DepositDate);
            Assert.AreEqual(doc.GetValue<String>("deposit_id"), dispute.DepositReference);
        }

        [TestMethod]
        public void MapSettlementDisputeSummaryTest_NullDate()
        {
            string rawJson = "{\"id\":\"DIS_812\",\"status\":\"FUNDED\",\"stage\":\"CHARGEBACK\",\"stage_time_created\":\"\",\"amount\":\"200\",\"currency\":\"GBP\",\"reason_code\":\"PM\",\"reason_description\":\"Paid by Other Means\",\"time_to_respond_by\":\"\",\"result\":\"LOST\",\"funding_type\":\"DEBIT\",\"deposit_time_created\":\"\",\"deposit_id\":\"DEP_2342423443\",\"last_adjustment_amount\":\"\",\"last_adjustment_currency\":\"\",\"last_adjustment_funding\":\"\",\"last_adjustment_time_created\":\"\",\"system\":{\"mid\":\"11112334\",\"tid\":\"22229876\",\"hierarchy\":\"000-00-000-000-000\",\"name\":\"MERCHANT ABC INC.\",\"dba\":\"MERCHANT XYZ INC.\"},\"transaction\":{\"time_created\":\"\",\"merchant_time_created\":\"\",\"type\":\"SALE\",\"amount\":\"200\",\"currency\":\"GBP\",\"reference\":\"28012076eb6M\",\"payment_method\":{\"card\":{\"masked_number_first6last4\":\"379132XXXXX1007\",\"arn\":\"71400011203688701393903\",\"brand\":\"AMEX\",\"authcode\":\"129623\",\"brand_reference\":\"MWE1P0JG80110\"}}}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            DisputeSummary dispute = GpApiMapping.MapSettlementDisputeSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), dispute.CaseId);
            Assert.IsNull(dispute.CaseIdTime);
            Assert.AreEqual(doc.GetValue<string>("status"), dispute.CaseStatus);
            Assert.AreEqual(doc.GetValue<string>("stage"), dispute.CaseStage);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), dispute.CaseAmount);
            Assert.AreEqual(doc.GetValue<string>("currency"), dispute.CaseCurrency);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_amount").ToAmount(), dispute.LastAdjustmentAmount);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_currency"), dispute.LastAdjustmentCurrency);
            Assert.AreEqual(doc.GetValue<string>("last_adjustment_funding"), dispute.LastAdjustmentFunding);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("mid"), dispute.CaseMerchantId);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("tid"), dispute.CaseTerminalId);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("hierarchy"), dispute.MerchantHierarchy);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("name"), dispute.MerchantName);
            Assert.AreEqual(doc.Get("system")?.GetValue<string>("dba"), dispute.MerchantDbaName);
            Assert.AreEqual(doc.GetValue<string>("reason_code"), dispute.ReasonCode);
            Assert.AreEqual(doc.GetValue<string>("reason_description"), dispute.Reason);
            Assert.AreEqual(doc.GetValue<string>("result"), dispute.Result);
            Assert.IsNull(dispute.TransactionTime);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<string>("type"), dispute.TransactionType);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<string>("amount").ToAmount(), dispute.TransactionAmount);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<string>("currency"), dispute.TransactionCurrency);
            Assert.AreEqual(doc.Get("transaction")?.GetValue<string>("reference"), dispute.TransactionReferenceNumber);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("masked_number_first6last4"), dispute.TransactionMaskedCardNumber);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("arn"), dispute.TransactionARN);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("brand"), dispute.TransactionCardType);
            Assert.AreEqual(doc.Get("transaction")?.Get("payment_method")?.Get("card")?.GetValue<string>("authcode"), dispute.TransactionAuthCode);
            Assert.IsNull(dispute.RespondByDate);
            Assert.IsNull(dispute.DepositDate);
            Assert.AreEqual(doc.GetValue<String>("deposit_id"), dispute.DepositReference);
        }

        [TestMethod]
        public void MapMapStoredPaymentMethodSummaryTest() {
            // Arrange
            string rawJson = "{\"id\":\"PMT_3502a05c-0a79-469b-bff9-994b665ce9d9\",\"time_created\":\"2021-04-23T18:46:57.000Z\",\"status\":\"ACTIVE\",\"merchant_id\":\"MER_c4c0df11039c48a9b63701adeaa296c3\",\"merchant_name\":\"Sandbox_merchant_2\",\"account_id\":\"TKA_eba30a1b5c4a468d90ceeef2ffff7f5e\",\"account_name\":\"Tokenization\",\"reference\":\"faed4ae3-1dd6-414a-bd7e-3a585715d9cc\",\"card\":{\"number_last4\":\"xxxxxxxxxxxx1111\",\"brand\":\"VISA\",\"expiry_month\":\"12\",\"expiry_year\":\"25\"},\"action\":{\"id\":\"ACT_wFGcHivudqleji9jA7S4MTapAHCTkp\",\"type\":\"PAYMENT_METHOD_SINGLE\",\"time_created\":\"2021-04-23T18:47:01.057Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg\",\"app_name\":\"colleens_app\"}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            StoredPaymentMethodSummary paymentMethod = GpApiMapping.MapStoredPaymentMethodSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), paymentMethod.Id);
            Assert.AreEqual(doc.GetValue<DateTime>("time_created"), paymentMethod.TimeCreated);
            Assert.AreEqual(doc.GetValue<string>("status"), paymentMethod.Status);
            Assert.AreEqual(doc.GetValue<string>("reference"), paymentMethod.Reference);
            Assert.AreEqual(doc.GetValue<string>("name"), paymentMethod.Name);
            Assert.AreEqual(doc.Get("card")?.GetValue<string>("number_last4"), paymentMethod.CardLast4);
            Assert.AreEqual(doc.Get("card")?.GetValue<string>("brand"), paymentMethod.CardType);
            Assert.AreEqual(doc.Get("card")?.GetValue<string>("expiry_month"), paymentMethod.CardExpMonth);
            Assert.AreEqual(doc.Get("card")?.GetValue<string>("expiry_year"), paymentMethod.CardExpYear);
        }

        [TestMethod]
        public void MapActionSummaryTest() {
            // Arrange
            string rawJson = "{\"id\":\"ACT_PJiFWTaNcLW8aVBo2fA8E5Dqd8ZyrH\",\"type\":\"CREATE_TOKEN\",\"time_created\":\"2021-03-24T02:02:27.158Z\",\"resource\":\"ACCESS_TOKENS\",\"resource_request_url\":\"http://localhost:8998/v7/unifiedcommerce/accesstoken\",\"version\":\"2020-12-22\",\"resource_parent_id\":\"\",\"resource_id\":\"ACT_PJiFWTaNcLW8aVBo2fA8E5Dqd8ZyrH\",\"resource_status\":\"\",\"http_response_code\":\"200\",\"http_response_message\":\"OK\",\"response_code\":\"SUCCESS\",\"response_detailed_code\":\"\",\"response_detailed_message\":\"\",\"app_id\":\"P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg\",\"app_name\":\"colleens_app\",\"app_developer\":\"colleen.mcgloin@globalpay.com\",\"merchant_id\":\"MER_c4c0df11039c48a9b63701adeaa296c3\",\"merchant_name\":\"Sandbox_merchant_2\",\"account_id\":\"\",\"account_name\":\"\",\"source_location\":\"63.241.252.2\",\"destination_location\":\"74.125.196.153\",\"metrics\":{\"X-GP-Version\":\"2020-12-22\"},\"action\":{\"id\":\"ACT_qOTwHG38UvuWwjcI6DBNu0uqbg8eoR\",\"type\":\"ACTION_SINGLE\",\"time_created\":\"2021-04-23T18:23:05.824Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg\",\"app_name\":\"colleens_app\"}}";

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Act
            ActionSummary action = GpApiMapping.MapActionSummary(doc);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), action.Id);
            Assert.AreEqual(doc.GetValue<string>("type"), action.Type);
            Assert.AreEqual(doc.GetValue<DateTime>("time_created"), action.TimeCreated);
            Assert.AreEqual(doc.GetValue<string>("resource"), action.Resource);
            Assert.AreEqual(doc.GetValue<string>("resource_request_url"), action.ResourceRequestUrl);
            Assert.AreEqual(doc.GetValue<string>("version"), action.Version);
            Assert.AreEqual(doc.GetValue<string>("resource_parent_id"), action.ResourceParentId);
            Assert.AreEqual(doc.GetValue<string>("resource_id"), action.ResourceId);
            Assert.AreEqual(doc.GetValue<string>("resource_status"), action.ResourceStatus);
            Assert.AreEqual(doc.GetValue<string>("http_response_code"), action.HttpResponseCode);
            Assert.AreEqual(doc.GetValue<string>("http_response_message"), action.HttpResponseMessage);
            Assert.AreEqual(doc.GetValue<string>("response_code"), action.ResponseCode);
            Assert.AreEqual(doc.GetValue<string>("response_detailed_code"), action.ResponseDetailedCode);
            Assert.AreEqual(doc.GetValue<string>("response_detailed_message"), action.ResponseDetailedMessage);
            Assert.AreEqual(doc.GetValue<string>("app_id"), action.AppId);
            Assert.AreEqual(doc.GetValue<string>("app_name"), action.AppName);
            Assert.AreEqual(doc.GetValue<string>("app_developer"), action.AppDeveloper);
            Assert.AreEqual(doc.GetValue<string>("merchant_id"), action.MerchantId);
            Assert.AreEqual(doc.GetValue<string>("account_id"), action.AccountId);
            Assert.AreEqual(doc.GetValue<string>("account_name"), action.AccountName);
            Assert.AreEqual(doc.GetValue<string>("merchant_name"), action.MerchantName);
            Assert.AreEqual(doc.GetValue<string>("source_location"), action.SourceLocation);
            Assert.AreEqual(doc.GetValue<string>("destination_location"), action.DestinationLocation);
            Assert.IsNotNull(action.Metrics);
            Assert.AreEqual(doc.Get("metrics").GetValue<string>("X-GP-Version"), action.Metrics["X-GP-Version"]);
        }

        [TestMethod]
        public void MapActionSummary_EmptyTimeCreated_MapsDefaultDate() {
            // Arrange
            const string rawJson = "{\"id\":\"\",\"type\":\"\",\"time_created\":\"\",\"resource\":\"\",\"resource_status\":\"CAPTURED\"}";

            // Act
            ActionSummary action = GpApiMapping.MapActionSummary(JsonDoc.Parse(rawJson));

            // Assert
            Assert.AreEqual(default(DateTime), action.TimeCreated);
            Assert.AreEqual("CAPTURED", action.ResourceStatus);
        }

        [TestMethod]
        public void MapReportResponse_FindActionsPaged_MapsEnvelopeAndActionRecords() {
            // Arrange
            const string rawJson = "{\"total_record_count\":1,\"current_page_size\":1,\"merchant_id\":\"MER_123\",\"merchant_name\":\"Sandbox_merchant\",\"filter\":{\"from_time_created\":\"2022-05-01\",\"to_time_created\":\"2023-05-01\"},\"paging\":{\"page\":1,\"page_size\":5,\"order_by\":\"TIME_CREATED\",\"order\":\"ASC\"},\"actions\":[{\"id\":\"ACT_123\",\"type\":\"MERCHANT_EDIT\",\"time_created\":\"2023-05-01T12:00:00.000Z\",\"resource\":\"TRANSACTIONS\",\"resource_id\":\"TRN_123\",\"resource_status\":\"DECLINED\",\"version\":\"2021-03-22\",\"http_response_code\":\"400\",\"response_code\":\"DECLINED\",\"app_id\":\"APP_123\",\"app_name\":\"test_app\",\"merchant_name\":\"Sandbox_merchant\",\"account_name\":\"transaction_processing\"}],\"action\":{\"id\":\"ACT_result\",\"type\":\"ACTION_LIST\",\"time_created\":\"2023-05-01T12:01:00.000Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"APP_123\",\"app_name\":\"test_app\"}}";

            // Act
            PagedResult<ActionSummary> result = GpApiMapping.MapReportResponse<PagedResult<ActionSummary>>(rawJson, ReportType.FindActionsPaged);

            // Assert
            Assert.AreEqual(1, result.TotalRecordCount);
            Assert.AreEqual(1, result.CurrentPageSize);
            Assert.AreEqual("MER_123", result.MerchantId);
            Assert.AreEqual("Sandbox_merchant", result.MerchantName);
            Assert.AreEqual(new DateTime(2022, 5, 1), result.FromTimeCreated.Value.Date);
            Assert.AreEqual(new DateTime(2023, 5, 1), result.ToTimeCreated.Value.Date);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(5, result.PageSize);
            Assert.AreEqual("TIME_CREATED", result.OrderBy);
            Assert.AreEqual("ASC", result.Order);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual("ACT_123", result.Results[0].Id);
            Assert.AreEqual("MERCHANT_EDIT", result.Results[0].Type);
            Assert.AreEqual("TRANSACTIONS", result.Results[0].Resource);
            Assert.AreEqual("TRN_123", result.Results[0].ResourceId);
            Assert.AreEqual("DECLINED", result.Results[0].ResourceStatus);
            Assert.AreEqual("2021-03-22", result.Results[0].Version);
            Assert.AreEqual("400", result.Results[0].HttpResponseCode);
            Assert.AreEqual("DECLINED", result.Results[0].ResponseCode);
            Assert.AreEqual("APP_123", result.Results[0].AppId);
            Assert.AreEqual("test_app", result.Results[0].AppName);
            Assert.AreEqual("Sandbox_merchant", result.Results[0].MerchantName);
            Assert.AreEqual("transaction_processing", result.Results[0].AccountName);
            Assert.IsNotNull(result.Action);
            Assert.AreEqual("ACT_result", result.Action.Id);
            Assert.AreEqual("ACTION_LIST", result.Action.Type);
            Assert.AreEqual("SUCCESS", result.Action.ResultCode);
            Assert.AreEqual("APP_123", result.Action.AppId);
            Assert.AreEqual("test_app", result.Action.AppName);
        }

        /// <summary>Maps authentication detail fields and paged-list envelope metadata.</summary>
        [TestMethod]
        public void MapReportResponse_Authentications_MapsDetailAndPagedEnvelope() {
            var detail = GpApiMapping.MapReportResponse<AuthenticationSummary>(
                AuthenticationResponseJson, ReportType.AuthenticationDetail);
            var listResponseJson = @"{
                'current_page_size':1,'merchant_id':'MER_contract','merchant_name':'Contract merchant',
                'account_id':'TRA_contract','account_name':'transaction_processing',
                'filter':{'from_time_created':'2026-01-01','to_time_created':'2026-01-31'},
                'paging':{'page_size':20,'page':2,'order':'DESC','order_by':'TIME_CREATED'},
                'authentications':" + AuthenticationResponseJson + @",
                'action':{'id':'ACT_list','type':'AUTHENTICATIONS_LIST',
                    'time_created':'2026-01-04T05:06:07.890Z','result_code':'SUCCESS'}}";
            var page = GpApiMapping.MapReportResponse<PagedResult<AuthenticationSummary>>(
                listResponseJson, ReportType.FindAuthenticationsPaged);

            Assert.AreEqual("AUT_contract", detail.Id);
            Assert.AreEqual("SALE", detail.TransactionType);
            Assert.AreEqual("AVAILABLE", detail.Status);
            Assert.AreEqual(12.34m, detail.Amount);
            Assert.AreEqual("TRA_contract", detail.AccountId);
            Assert.AreEqual("authentication_reference", detail.Reference);
            Assert.AreEqual("Test card", detail.PaymentMethod.Name);
            Assert.AreEqual("fingerprint", detail.PaymentMethod.Fingerprint);
            Assert.AreEqual("AUTH", detail.PaymentMethod.Card.AuthCode);
            Assert.AreEqual("411111xxxx1111", detail.PaymentMethod.Card.MaskedNumberFirst6Last4);
            Assert.AreEqual("AUTHENTICATION_SUCCESSFUL", detail.PaymentMethod.ThreeDSecure.Status);
            Assert.AreEqual("AUTHENTICATION_SUCCESSFUL", detail.ThreeDSecure.Status);
            Assert.AreEqual("NOT_MANDATED", detail.ThreeDSecure.ChallengeStatus);
            Assert.AreEqual("2.2.0", detail.ThreeDSecure.MessageVersion);
            Assert.AreEqual("STATIC_CHALLENGE", detail.ThreeDSecure.ChallengeModel);
            Assert.AreEqual("challenge-value", detail.ThreeDSecure.ChallengeValue);
            Assert.AreEqual("https://acs.example/challenge", detail.ThreeDSecure.AcsChallengeRequestUrl);
            Assert.AreEqual("METHOD_TRANS", detail.ThreeDSecure.MethodData.ThreeDSServerTransactionId);
            Assert.AreEqual("signed-content", detail.ThreeDSecure.MobileData.AcsSignedContent);
            Assert.AreEqual("NATIVE", detail.ThreeDSecure.MobileData.AcsRenderingType.AcsInterface);
            Assert.AreEqual("https://example.com/challenge-return", detail.Notifications.ChallengeReturnUrl);
            Assert.AreEqual("ACT_CREATE", detail.ActionCreateId);
            Assert.AreEqual("TID_contract", detail.System.Tid);
            Assert.AreEqual("ACT_auth", detail.Action.Id);

            Assert.AreEqual(1, page.CurrentPageSize);
            Assert.AreEqual("MER_contract", page.MerchantId);
            Assert.AreEqual("TRA_contract", page.AccountId);
            Assert.AreEqual(new DateTime(2026, 1, 1), page.FromTimeCreated.Value.Date);
            Assert.AreEqual(new DateTime(2026, 1, 31), page.ToTimeCreated.Value.Date);
            Assert.AreEqual(2, page.Page);
            Assert.AreEqual(20, page.PageSize);
            Assert.AreEqual("DESC", page.Order);
            Assert.AreEqual(1, page.Results.Count);
            Assert.AreEqual("AUT_contract", page.Results[0].Id);
            Assert.AreEqual("ACT_list", page.Action.Id);

            var transaction = GpApiMapping.Map3DSecureData(AuthenticationResponseJson);
            Assert.AreEqual("AUT_contract", transaction.TransactionId);
            Assert.AreEqual("MER_contract", transaction.MerchantId);
            Assert.AreEqual("TRA_contract", transaction.AccountId);
            Assert.AreEqual("AUT_contract", transaction.AuthenticationDetails.Id);
            Assert.AreEqual("https://acs.example/redirect", transaction.AuthenticationDetails.ThreeDSecure.RedirectUrl);
        }

        /// <summary>Maps the documented scalar mobile rendering type and flat mobile response fields.</summary>
        [TestMethod]
        public void MapAuthentication_WithFlatMobileData_ReturnsDocumentedFields() {
            const string response = @"{
                'id':'AUT_mobile','status':'CHALLENGE_REQUIRED','source':'MOBILE_SDK',
                'three_ds':{'message_version':'2.2.0','status':'CHALLENGE_REQUIRED',
                    'mobile_data':{'acs_signed_content':'signed-content','acs_rendering_type':'NATIVE',
                        'acs_interface':'HTML','acs_ui_template':'MULTI_SELECT'}}}";

            var transaction = GpApiMapping.Map3DSecureData(response);
            var threeDS = transaction.AuthenticationDetails.ThreeDSecure;

            Assert.AreEqual("CHALLENGE_REQUIRED", transaction.Status);
            Assert.IsNotNull(threeDS);
            Assert.AreEqual("HTML", threeDS.AcsInterface);
            Assert.AreEqual("MULTI_SELECT", threeDS.AcsUiTemplate);
            Assert.IsNotNull(threeDS.MobileData);
            Assert.AreEqual("signed-content", threeDS.MobileData.AcsSignedContent);
            Assert.AreEqual("NATIVE", threeDS.MobileData.AcsRenderingTypeValue);
            Assert.AreEqual("HTML", threeDS.MobileData.AcsInterface);
            Assert.AreEqual("MULTI_SELECT", threeDS.MobileData.AcsUiTemplate);
            Assert.IsNull(threeDS.MobileData.AcsRenderingType);
        }

        /// <summary>Maps whitelist status and the documented result string arrays.</summary>
        [TestMethod]
        public void MapAuthentication_WithResultArrays_ReturnsWhitelistAndExtensions() {
            const string response = @"{
                'id':'AUT_result','status':'SUCCESS_AUTHENTICATED',
                'three_ds':{'status':'AUTHENTICATION_SUCCESSFUL',
                    'whitelist_status':'WHITELISTED_BY_CARDHOLDER',
                    'acs_decoupled_response_indicator':['DECOUPLED_AUTHENTICATION_UTILISED'],
                    'message_extension':['extension-one','extension-two']}}";

            var transaction = GpApiMapping.Map3DSecureData(response);
            var details = transaction.AuthenticationDetails;

            Assert.AreEqual("SUCCESS_AUTHENTICATED", transaction.Status);
            Assert.AreEqual("SUCCESS_AUTHENTICATED", details.Status);
            Assert.AreEqual("AUTHENTICATION_SUCCESSFUL", details.ThreeDSecure.Status);
            Assert.AreEqual("WHITELISTED_BY_CARDHOLDER", details.ThreeDSecure.WhitelistStatus);
            CollectionAssert.AreEqual(new[] { "DECOUPLED_AUTHENTICATION_UTILISED" },
                new List<string>(details.ThreeDSecure.AcsDecoupledResponseIndicator));
            CollectionAssert.AreEqual(new[] { "extension-one", "extension-two" },
                new List<string>(details.ThreeDSecure.MessageExtensionValues));
        }

        /// <summary>Maps all newly supported authentication entry modes to their documented wire values.</summary>
        [DataTestMethod]
        [DataRow(PaymentEntryMode.Phone, "PHONE")]
        [DataRow(PaymentEntryMode.Mail, "MAIL")]
        [DataRow(PaymentEntryMode.Present, "PRESENT")]
        [DataRow(PaymentEntryMode.Unknown, "UNKNOWN")]
        public void MapAuthenticationEntryMode_ReturnsDocumentedValue(PaymentEntryMode mode, string expected) {
            Assert.AreEqual(expected, EnumConverter.GetMapping(Target.GP_API, mode));
        }

        /// <summary>Maps all newly supported authentication stored-credential models.</summary>
        [DataTestMethod]
        [DataRow(StoredCredentialType.AddPaymentMethod, "ADD_PAYMENT_METHOD")]
        [DataRow(StoredCredentialType.SplitOrDelayedShipment, "SPLIT_OR_DELAYED_SHIPMENT")]
        [DataRow(StoredCredentialType.TopUp, "TOP_UP")]
        [DataRow(StoredCredentialType.MailOrder, "MAIL_ORDER")]
        [DataRow(StoredCredentialType.TelephoneOrder, "TELEPHONE_ORDER")]
        [DataRow(StoredCredentialType.WhitelistStatusCheck, "WHITELIST_STATUS_CHECK")]
        [DataRow(StoredCredentialType.OtherPayment, "OTHER_PAYMENT")]
        [DataRow(StoredCredentialType.BillingAgreement, "BILLING_AGREEMENT")]
        public void MapAuthenticationStoredCredential_ReturnsDocumentedValue(StoredCredentialType model, string expected) {
            Assert.AreEqual(expected, EnumConverter.GetMapping(Target.GP_API, model));
        }

        [TestMethod]
        public void MapResponseTest_CreateTransaction() {
            // Arrange
            string rawJson = "{\"id\":\"TRN_BHZ1whvNJnMvB6dPwf3znwWTsPjCn0\",\"time_created\":\"2020-12-04T12:46:05.235Z\",\"type\":\"SALE\",\"status\":\"PREAUTHORIZED\",\"channel\":\"CNP\",\"capture_mode\":\"LATER\",\"amount\":\"1400\",\"currency\":\"USD\",\"country\":\"US\",\"merchant_id\":\"MER_c4c0df11039c48a9b63701adeaa296c3\",\"merchant_name\":\"Sandbox_merchant_2\",\"account_id\":\"TRA_6716058969854a48b33347043ff8225f\",\"account_name\":\"Transaction_Processing\",\"reference\":\"15fbcdd9-8626-4e29-aae8-050f823f995f\",\"payment_method\":{\"id\":\"PMT_9a8f1b66-58e3-409d-86df-ed5fb14ad2f6\",\"result\":\"00\",\"message\":\"[ test system ] AUTHORISED\",\"entry_mode\":\"ECOM\",\"card\":{\"brand\":\"VISA\",\"masked_number_last4\":\"XXXXXXXXXXXX5262\",\"authcode\":\"12345\",\"brand_reference\":\"PSkAnccWLNMTcRmm\",\"brand_time_created\":\"\",\"cvv_result\":\"MATCHED\"}},\"batch_id\":\"\",\"action\":{\"id\":\"ACT_BHZ1whvNJnMvB6dPwf3znwWTsPjCn0\",\"type\":\"PREAUTHORIZE\",\"time_created\":\"2020-12-04T12:46:05.235Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"Uyq6PzRbkorv2D4RQGlldEtunEeGNZll\",\"app_name\":\"sample_app_CERT\"}}";

            // Act
            Transaction transaction = GpApiMapping.MapResponse(rawJson);

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), transaction.TransactionId);
            Assert.AreEqual(14m, transaction.AuthorizedAmount);
            Assert.AreEqual(doc.GetValue<string>("time_created"), transaction.Timestamp);
            Assert.AreEqual(doc.GetValue<string>("status"), transaction.ResponseMessage);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ReferenceNumber);
            Assert.AreEqual(doc.GetValue<string>("batch_id"), transaction.BatchSummary?.BatchReference);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(doc.Get("payment_method")?.GetValue<string>("id"), transaction.Token);
            Assert.AreEqual(doc.Get("payment_method")?.GetValue<string>("result"), transaction.CardIssuerResponse.Result);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("authcode"), transaction.AuthorizationCode);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("brand"), transaction.CardType);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("masked_number_last4"), transaction.CardLast4);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("cvv_result"), transaction.CvnResponseMessage);
        }

        [TestMethod]
        public void MapResponseTest_CreateTransaction_withAvsData()
        {
            // Arrange
            string rawJson = "{\"id\":\"TRN_J7ocSiyeHOJ1XK1jHjg9hq9U5nS0Nz_057d45d5f1fc\",\"time_created\":\"2021-09-01T14:27:41.713Z\",\"type\":\"SALE\",\"status\":\"CAPTURED\",\"channel\":\"CNP\",\"capture_mode\":\"AUTO\",\"amount\":\"1999\",\"currency\":\"USD\",\"country\":\"US\",\"merchant_id\":\"MER_7e3e2c7df34f42819b3edee31022ee3f\",\"merchant_name\":\"Sandbox_merchant_3\",\"account_id\":\"TRA_c9967ad7d8ec4b46b6dd44a61cde9a91\",\"account_name\":\"transaction_processing\",\"reference\":\"4d361180-304a-4f8a-9e82-057d45d5f1fc\",\"payment_method\":{\"result\":\"00\",\"message\":\"[ test system ] AUTHORISED\",\"entry_mode\":\"ECOM\",\"fingerprint\":\"\",\"fingerprint_presence_indicator\":\"\",\"card\":{\"funding\":\"CREDIT\",\"brand\":\"VISA\",\"masked_number_last4\":\"XXXXXXXXXXXX5262\",\"authcode\":\"12345\",\"brand_reference\":\"vQBOsL3WUjuaaEmT\",\"brand_time_created\":\"\",\"cvv_result\":\"MATCHED\",\"avs_address_result\":\"MATCHED\",\"avs_postal_code_result\":\"MATCHED\",\"avs_action\":\"\",\"provider\":{\"result\":\"00\",\"cvv_result\":\"M\",\"avs_address_result\":\"M\",\"avs_postal_code_result\":\"M\"}}},\"batch_id\":\"BAT_983471\",\"action\":{\"id\":\"ACT_J7ocSiyeHOJ1XK1jHjg9hq9U5nS0Nz\",\"type\":\"AUTHORIZE\",\"time_created\":\"2021-09-01T14:27:41.713Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"rkiYguPfTurmGcVhkDbIGKn2IJe2t09M\",\"app_name\":\"sample_app_CERT\"}}";

            // Act
            Transaction transaction = GpApiMapping.MapResponse(rawJson);

            JsonDoc doc = JsonDoc.Parse(rawJson);

            // Assert
            Assert.AreEqual(doc.GetValue<string>("id"), transaction.TransactionId);
            Assert.AreEqual(19.99m, transaction.AuthorizedAmount);
            Assert.AreEqual(doc.GetValue<string>("time_created"), transaction.Timestamp);
            Assert.AreEqual(doc.GetValue<string>("status"), transaction.ResponseMessage);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ReferenceNumber);
            Assert.AreEqual(doc.GetValue<string>("batch_id"), transaction.BatchSummary?.BatchReference);
            Assert.AreEqual("00", transaction.ResponseCode);
            Assert.AreEqual(doc.Get("payment_method")?.GetValue<string>("id"), transaction.Token);
            Assert.AreEqual(doc.Get("payment_method")?.GetValue<string>("result"), transaction.CardIssuerResponse.Result);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("authcode"), transaction.AuthorizationCode);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("brand"), transaction.CardType);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("masked_number_last4"), transaction.CardLast4);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("cvv_result"), transaction.CvnResponseMessage);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("avs_postal_code_result"), transaction.AvsResponseCode);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("avs_address_result"), transaction.AvsAddressResponse);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("avs_action"), transaction.AvsResponseMessage);
        }

        [TestMethod]
        public void MapResponseTest_BatchClose() {
            // Arrange
            string rawJson = "{\"id\":\"BAT_631762-460\",\"time_last_updated\":\"2021-04-23T18:54:52.467Z\",\"status\":\"CLOSED\",\"amount\":\"869\",\"currency\":\"USD\",\"country\":\"US\",\"transaction_count\":2,\"action\":{\"id\":\"ACT_QUuw7OPd9Rw8n72oaVOmVlQXpuhLUZ\",\"type\":\"CLOSE\",\"time_created\":\"2021-04-23T18:54:52.467Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg\",\"app_name\":\"colleens_app\"}}";

            // Act
            Transaction transaction = GpApiMapping.MapResponse(rawJson);

            // Assert
            Assert.IsNotNull(transaction?.BatchSummary);
            JsonDoc doc = JsonDoc.Parse(rawJson);
            Assert.AreEqual(doc.GetValue<string>("id"), transaction?.BatchSummary?.BatchReference);
            Assert.AreEqual(doc.GetValue<string>("status"), transaction?.BatchSummary?.Status);
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), transaction?.BatchSummary?.TotalAmount);
            Assert.AreEqual(doc.GetValue<int>("transaction_count"), transaction?.BatchSummary?.TransactionCount);
        }

        /// <summary>
        /// Verifies that all batch response fields, including nested brand and host totals, are mapped.
        /// </summary>
        [TestMethod]
        public void MapReportResponse_BatchById_MapsAllFields() {
            const string rawJson = @"{
                ""id"": ""BAT_123"",
                ""status"": ""CLOSED"",
                ""time_created"": ""2024-01-31T13:33:00Z"",
                ""time_last_updated"": ""2024-01-31T18:08:29.307Z"",
                ""time_closed"": ""2024-01-31T18:08:00Z"",
                ""open_action_id"": ""ACT_OPEN"",
                ""close_action_id"": ""ACT_CLOSE"",
                ""merchant_id"": ""MER_123"",
                ""merchant_name"": ""Sandbox Merchant"",
                ""account_id"": ""TRA_123"",
                ""account_name"": ""Transaction Processing"",
                ""country"": ""US"",
                ""site_reference"": ""site-1"",
                ""device_reference"": ""device-1"",
                ""batch_transaction_count"": 48,
                ""transaction_count"": 47,
                ""amount"": ""31396"",
                ""currency"": ""USD"",
                ""gratuity_amount"": ""100"",
                ""sales"": { ""count"": 36, ""amount"": ""45012"" },
                ""refunds"": { ""count"": 12, ""amount"": ""-13616"" },
                ""funding_debit"": { ""count"": 1, ""amount"": ""500"" },
                ""funding_credit"": { ""count"": 47, ""amount"": ""30896"" },
                ""brand_breakdown"": [
                    {
                        ""brand"": ""VISA"",
                        ""count"": 47,
                        ""amount"": ""31296"",
                        ""gratuity_amount"": ""100"",
                        ""sales"": { ""count"": 35, ""amount"": ""44912"" },
                        ""refunds"": { ""count"": 12, ""amount"": ""-13616"" },
                        ""funding_debit"": { ""count"": 0, ""amount"": ""0"" },
                        ""funding_credit"": { ""count"": 47, ""amount"": ""31296"" }
                    }
                ],
                ""host_breakdown"": {
                    ""merchant_name"": ""GP API CP"",
                    ""reference"": ""2323"",
                    ""count"": 48,
                    ""amount"": ""31396"",
                    ""funding_debit"": {
                        ""sales"": { ""count"": 0, ""amount"": ""0"" },
                        ""refunds"": { ""count"": 0, ""amount"": ""0"" }
                    },
                    ""funding_credit"": {
                        ""sales"": { ""count"": 36, ""amount"": ""45012"" },
                        ""refunds"": { ""count"": 12, ""amount"": ""-13616"" }
                    }
                },
                ""action"": {
                    ""id"": ""ACT_CLOSE"",
                    ""type"": ""CLOSE"",
                    ""time_created"": ""2024-01-31T18:08:29.307Z"",
                    ""result_code"": ""SUCCESS"",
                    ""app_id"": ""APP_123"",
                    ""app_name"": ""batch-app""
                }
            }";

            var summary = GpApiMapping.MapReportResponse<BatchSummary>(rawJson, ReportType.BatchById);

            Assert.AreEqual("BAT_123", summary.BatchReference);
            Assert.AreEqual("CLOSED", summary.Status);
            Assert.AreEqual(new DateTime(2024, 1, 31, 13, 33, 0), summary.TimeCreated);
            Assert.AreEqual(new DateTime(2024, 1, 31, 18, 8, 29, 307), summary.TimeLastUpdated);
            Assert.AreEqual(new DateTime(2024, 1, 31, 18, 8, 0), summary.TimeClosed);
            Assert.AreEqual("ACT_OPEN", summary.OpenActionId);
            Assert.AreEqual("ACT_CLOSE", summary.CloseActionId);
            Assert.AreEqual("MER_123", summary.MerchantId);
            Assert.AreEqual("Sandbox Merchant", summary.MerchantName);
            Assert.AreEqual("TRA_123", summary.AccountId);
            Assert.AreEqual("Transaction Processing", summary.AccountName);
            Assert.AreEqual("US", summary.Country);
            Assert.AreEqual("site-1", summary.SiteReference);
            Assert.AreEqual("device-1", summary.DeviceReference);
            Assert.AreEqual(48, summary.BatchTransactionCount);
            Assert.AreEqual(47, summary.TransactionCount);
            Assert.AreEqual(313.96m, summary.TotalAmount);
            Assert.AreEqual("USD", summary.Currency);
            Assert.AreEqual(1m, summary.GratuityAmount);
            Assert.AreEqual(36, summary.SaleCount);
            Assert.AreEqual(450.12m, summary.SaleAmount);
            Assert.AreEqual(12, summary.ReturnCount);
            Assert.AreEqual(-136.16m, summary.ReturnAmount);
            Assert.AreEqual(1, summary.DebitCount);
            Assert.AreEqual(5m, summary.DebitAmount);
            Assert.AreEqual(47, summary.CreditCount);
            Assert.AreEqual(308.96m, summary.CreditAmount);

            Assert.IsNotNull(summary.BrandBreakdown);
            Assert.AreEqual(1, summary.BrandBreakdown.Count);
            var brand = summary.BrandBreakdown[0];
            Assert.AreEqual("VISA", brand.Brand);
            Assert.AreEqual(47, brand.Count);
            Assert.AreEqual(312.96m, brand.Amount);
            Assert.AreEqual(1m, brand.GratuityAmount);
            Assert.AreEqual(35, brand.Sales.Count);
            Assert.AreEqual(449.12m, brand.Sales.Amount);
            Assert.AreEqual(12, brand.Refunds.Count);
            Assert.AreEqual(-136.16m, brand.Refunds.Amount);
            Assert.AreEqual(0, brand.FundingDebit.Count);
            Assert.AreEqual(0m, brand.FundingDebit.Amount);
            Assert.AreEqual(47, brand.FundingCredit.Count);
            Assert.AreEqual(312.96m, brand.FundingCredit.Amount);

            Assert.IsNotNull(summary.HostBreakdown);
            Assert.AreEqual("GP API CP", summary.HostBreakdown.MerchantName);
            Assert.AreEqual("2323", summary.HostBreakdown.Reference);
            Assert.AreEqual(48, summary.HostBreakdown.Count);
            Assert.AreEqual(313.96m, summary.HostBreakdown.Amount);
            Assert.AreEqual(0, summary.HostBreakdown.FundingDebit.Sales.Count);
            Assert.AreEqual(0m, summary.HostBreakdown.FundingDebit.Sales.Amount);
            Assert.AreEqual(36, summary.HostBreakdown.FundingCredit.Sales.Count);
            Assert.AreEqual(450.12m, summary.HostBreakdown.FundingCredit.Sales.Amount);
            Assert.AreEqual(12, summary.HostBreakdown.FundingCredit.Refunds.Count);
            Assert.AreEqual(-136.16m, summary.HostBreakdown.FundingCredit.Refunds.Amount);

            Assert.IsNotNull(summary.Action);
            Assert.AreEqual("ACT_CLOSE", summary.Action.Id);
            Assert.AreEqual("CLOSE", summary.Action.Type);
            Assert.IsFalse(string.IsNullOrEmpty(summary.Action.TimeCreated));
            Assert.AreEqual("SUCCESS", summary.Action.ResultCode);
            Assert.AreEqual("APP_123", summary.Action.AppId);
            Assert.AreEqual("batch-app", summary.Action.AppName);
        }

        /// <summary>
        /// Verifies that transactions and response metadata from a batch transaction query are mapped.
        /// </summary>
        [TestMethod]
        public void MapReportResponse_BatchTransactions_MapsResultsAndPaging() {
            const string rawJson = @"{
                ""total_record_count"": 1,
                ""current_page_size"": 1,
                ""batch_transaction_count"": 7,
                ""account_id"": ""TRA_123"",
                ""account_name"": ""processing"",
                ""site_reference"": ""site-123"",
                ""device_reference"": ""device-123"",
                ""filter"": { ""batch_id"": ""BAT_FILTER"" },
                ""merchant_id"": ""MER_123"",
                ""merchant_name"": ""Sandbox Merchant"",
                ""paging"": { ""page"": 1, ""page_size"": 10, ""order"": ""ASC"", ""order_by"": ""TIME_CREATED"" },
                ""action"": { ""id"": ""ACT_LIST"", ""type"": ""BATCH_LIST"", ""result_code"": ""SUCCESS"" },
                ""transactions"": [
                    {
                        ""id"": ""TRN_123"",
                        ""time_created"": ""2024-01-31T13:33:00Z"",
                        ""status"": ""CAPTURED"",
                        ""type"": ""SALE"",
                        ""amount"": ""1234"",
                        ""tax_amount"": ""123"",
                        ""gratuity_amount"": ""200"",
                        ""surcharge_amount"": ""50"",
                        ""cashback_amount"": ""300"",
                        ""payment_method"": {
                            ""card"": {
                                ""cvv_result"": ""MATCHED"",
                                ""avs_address"": ""123 Test Street"",
                                ""avs_address_result"": ""MATCHED"",
                                ""avs_postal_code_result"": ""NOT_MATCHED""
                            }
                        },
                        ""currency"": ""USD"",
                        ""batch_id"": ""BAT_123"",
                        ""country"": ""US""
                    }
                ]
            }";

            var result = GpApiMapping.MapReportResponse<PagedResult<TransactionSummary>>(
                rawJson, ReportType.BatchTransactions);

            Assert.AreEqual(1, result.TotalRecordCount);
            Assert.AreEqual(1, result.CurrentPageSize);
            Assert.AreEqual("MER_123", result.MerchantId);
            Assert.AreEqual("Sandbox Merchant", result.MerchantName);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(10, result.PageSize);
            Assert.AreEqual("ASC", result.Order);
            Assert.AreEqual("TIME_CREATED", result.OrderBy);
            Assert.IsNotNull(result.Action);
            Assert.AreEqual("ACT_LIST", result.Action.Id);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual("TRN_123", result.Results[0].TransactionId);
            Assert.AreEqual(12.34m, result.Results[0].Amount);
            Assert.AreEqual("USD", result.Results[0].Currency);
            Assert.AreEqual("BAT_123", result.Results[0].BatchId);
            Assert.AreEqual("BAT_123", result.Results[0].BatchSequenceNumber);
            Assert.AreEqual(7, result.BatchTransactionCount);
            Assert.AreEqual("TRA_123", result.AccountId);
            Assert.AreEqual("processing", result.AccountName);
            Assert.AreEqual("site-123", result.SiteReference);
            Assert.AreEqual("device-123", result.DeviceReference);
            Assert.AreEqual("BAT_FILTER", result.BatchId);
            var transaction = result.Results[0];
            Assert.AreEqual(1.23m, transaction.TaxAmount);
            Assert.AreEqual(2m, transaction.GratuityAmount);
            Assert.AreEqual(0.50m, transaction.SurchargeAmount);
            Assert.AreEqual(3m, transaction.CashBackAmount);
            Assert.AreEqual("MATCHED", transaction.CvvResult);
            Assert.AreEqual("123 Test Street", transaction.AvsAddress);
            Assert.AreEqual("MATCHED", transaction.AvsAddressResult);
            Assert.AreEqual("NOT_MATCHED", transaction.AvsPostalCodeResult);
        }

        /// <summary>
        /// Verifies missing optional batch transaction fields remain absent.
        /// </summary>
        [TestMethod]
        public void MapReportResponse_BatchTransactions_WithMissingOptionalFields() {
            var result = GpApiMapping.MapReportResponse<PagedResult<TransactionSummary>>(
                @"{""transactions"":[{""id"":""TRN_optional""}]}", ReportType.BatchTransactions);
            Assert.IsNull(result.BatchTransactionCount);
            Assert.IsNull(result.BatchId);
            Assert.IsNull(result.AccountId);
            Assert.IsNull(result.AccountName);
            Assert.IsNull(result.SiteReference);
            Assert.IsNull(result.DeviceReference);
            Assert.AreEqual(1, result.Results.Count);
            var transaction = result.Results[0];
            Assert.AreEqual("TRN_optional", transaction.TransactionId);
            Assert.IsNull(transaction.Amount);
            Assert.IsNull(transaction.TaxAmount);
            Assert.IsNull(transaction.GratuityAmount);
            Assert.IsNull(transaction.SurchargeAmount);
            Assert.IsNull(transaction.CashBackAmount);
            Assert.IsNull(transaction.CvvResult);
            Assert.IsNull(transaction.AvsAddress);
            Assert.IsNull(transaction.AvsAddressResult);
            Assert.IsNull(transaction.AvsPostalCodeResult);
        }

        /// <summary>
        /// Verifies batch transaction amounts respect currency precision and preserve zero values.
        /// </summary>
        /// <param name="currency">The response currency.</param>
        /// <param name="expected">The expected amount in major units.</param>
        [DataTestMethod]
        [DataRow("USD", "1.23")]
        [DataRow("KRW", "123")]
        [DataRow("JPY", "1.23")]
        [DataRow("BHD", "0.123")]
        public void MapReportResponse_BatchTransactions_WithCurrencyPrecision(string currency, string expected) {
            var rawJson = @"{""batch_transaction_count"":0,""filter"":null,""transactions"":[{
                ""amount"":""123"",""tax_amount"":""123"",""gratuity_amount"":""123"",
                ""surcharge_amount"":""123"",""cashback_amount"":""0"",
                ""payment_method"":{""card"":null},""currency"":""" + currency + @"""}]}";
            var result = GpApiMapping.MapReportResponse<PagedResult<TransactionSummary>>(
                rawJson, ReportType.BatchTransactions);
            Assert.AreEqual(0, result.BatchTransactionCount);
            Assert.IsNull(result.BatchId);
            Assert.AreEqual(1, result.Results.Count);
            var transaction = result.Results[0];
            var amount = decimal.Parse(expected, CultureInfo.InvariantCulture);
            Assert.AreEqual(amount, transaction.Amount);
            Assert.AreEqual(amount, transaction.TaxAmount);
            Assert.AreEqual(amount, transaction.GratuityAmount);
            Assert.AreEqual(amount, transaction.SurchargeAmount);
            Assert.AreEqual(0m, transaction.CashBackAmount);
            Assert.IsNull(transaction.CvvResult);
            Assert.IsNull(transaction.AvsAddress);
            Assert.IsNull(transaction.AvsAddressResult);
            Assert.IsNull(transaction.AvsPostalCodeResult);
        }

        [TestMethod]
        public void MapResponseTest_CreateStoredPaymentMethod() {
            // Arrange
            string rawJson = "{\"id\":\"PMT_e150ba7c-bbbd-41fe-bc04-f21d18def2a1\",\"time_created\":\"2021-04-26T14:59:00.813Z\",\"status\":\"ACTIVE\",\"usage_mode\":\"MULTIPLE\",\"merchant_id\":\"MER_c4c0df11039c48a9b63701adeaa296c3\",\"merchant_name\":\"Sandbox_merchant_2\",\"account_id\":\"TKA_eba30a1b5c4a468d90ceeef2ffff7f5e\",\"account_name\":\"Tokenization\",\"reference\":\"9486a9e8-d8bd-4fd2-877c-796d07f3a2ce\",\"card\":{\"masked_number_last4\":\"XXXXXXXXXXXX1111\",\"brand\":\"VISA\",\"expiry_month\":\"12\",\"expiry_year\":\"25\"},\"action\":{\"id\":\"ACT_jFOurWcX9CvA8UKtEywVpxArNEryvZ\",\"type\":\"PAYMENT_METHOD_CREATE\",\"time_created\":\"2021-04-26T14:59:00.813Z\",\"result_code\":\"SUCCESS\",\"app_id\":\"P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg\",\"app_name\":\"colleens_app\"}}";

            // Act
            Transaction transaction = GpApiMapping.MapResponse(rawJson);

            // Assert
            JsonDoc doc = JsonDoc.Parse(rawJson);
            Assert.AreEqual(doc.GetValue<string>("id"), transaction.Token);
            Assert.AreEqual(doc.GetValue<string>("time_created"), transaction.Timestamp);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ReferenceNumber);
            Assert.AreEqual(doc.Get("card")?.GetValue<string>("brand"), transaction.CardType);
            Assert.AreEqual(doc.Get("card")?.GetValue<string>("number"), transaction.CardNumber);
            Assert.AreEqual(doc.Get("card")?.GetValue<string>("masked_number_last4"), transaction.CardLast4);
            Assert.AreEqual(doc.Get("card")?.GetValue<int>("expiry_month"), transaction.CardExpMonth);
            Assert.AreEqual(doc.Get("card")?.GetValue<int>("expiry_year"), transaction.CardExpYear);
        }
    }
}
