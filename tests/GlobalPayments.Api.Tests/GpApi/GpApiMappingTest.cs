using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Mapping;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Dynamic;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiMappingTest {
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
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("number"), dispute.TransactionMaskedCardNumber);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("arn"), dispute.TransactionARN);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("brand"), dispute.TransactionCardType);
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
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("number"), dispute.TransactionMaskedCardNumber);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("arn"), dispute.TransactionARN);
            Assert.AreEqual(doc.Get("payment_method")?.Get("card")?.GetValue<string>("brand"), dispute.TransactionCardType);
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
            Assert.AreEqual(doc.GetValue<string>("version"), action.Version);
            Assert.AreEqual(doc.GetValue<string>("resource_id"), action.ResourceId);
            Assert.AreEqual(doc.GetValue<string>("resource_status"), action.ResourceStatus);
            Assert.AreEqual(doc.GetValue<string>("http_response_code"), action.HttpResponseCode);
            Assert.AreEqual(doc.GetValue<string>("response_code"), action.ResponseCode);
            Assert.AreEqual(doc.GetValue<string>("app_id"), action.AppId);
            Assert.AreEqual(doc.GetValue<string>("app_name"), action.AppName);
            Assert.AreEqual(doc.GetValue<string>("account_id"), action.AccountId);
            Assert.AreEqual(doc.GetValue<string>("account_name"), action.AccountName);
            Assert.AreEqual(doc.GetValue<string>("merchant_name"), action.MerchantName);
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
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), transaction.BalanceAmount);
            Assert.AreEqual(doc.GetValue<string>("time_created"), transaction.Timestamp);
            Assert.AreEqual(doc.GetValue<string>("status"), transaction.ResponseMessage);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ReferenceNumber);
            Assert.AreEqual(doc.GetValue<string>("batch_id"), transaction.BatchSummary?.BatchReference);
            Assert.AreEqual(doc.Get("action").GetValue<string>("result_code"), transaction.ResponseCode);
            Assert.AreEqual(doc.Get("payment_method")?.GetValue<string>("id"), transaction.Token);
            Assert.AreEqual(doc.Get("payment_method")?.GetValue<string>("result"), transaction.AuthorizationCode);
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
            Assert.AreEqual(doc.GetValue<string>("amount").ToAmount(), transaction.BalanceAmount);
            Assert.AreEqual(doc.GetValue<string>("time_created"), transaction.Timestamp);
            Assert.AreEqual(doc.GetValue<string>("status"), transaction.ResponseMessage);
            Assert.AreEqual(doc.GetValue<string>("reference"), transaction.ReferenceNumber);
            Assert.AreEqual(doc.GetValue<string>("batch_id"), transaction.BatchSummary?.BatchReference);
            Assert.AreEqual(doc.Get("action").GetValue<string>("result_code"), transaction.ResponseCode);
            Assert.AreEqual(doc.Get("payment_method")?.GetValue<string>("id"), transaction.Token);
            Assert.AreEqual(doc.Get("payment_method")?.GetValue<string>("result"), transaction.AuthorizationCode);
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
