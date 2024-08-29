using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.Terminals;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests {
    [TestClass]
    public class PorticoCreditTests {
        private IncrementalNumberProvider _generator;

        CreditCardData card;
        CreditTrackData track;
        CreditCardData VisaManual { get; set; }
        CreditCardData MasterCardManual{ get; set; }
        Address Address { get; set; }
        string ClientTransactionId {
            get {
                if (_generator == null) {
                    _generator = IncrementalNumberProvider.GetInstance();
                }
                return _generator.GetRequestId().ToString();
            }
        }

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w",
                IsSafDataSupported = true
            });

            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardHolderName = " UnitTest "
            };

            track = new CreditTrackData
            {
                Value = "<E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|>;",
                EncryptionData = new EncryptionData
                {
                    Version = "01"
                },
                EntryMethod = EntryMethod.Proximity
            };

            Address = new Address {
                StreetAddress1 = "8320",
                PostalCode = "85284"
            };

            MasterCardManual = new CreditCardData {
                Number = "5146315000000055",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "998"
            };

            VisaManual = new CreditCardData {
                Number = "4012000098765439",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "999"
            };
        }

        [TestMethod]
        public void CreditAuthorization() {
            var response = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var capture = response.Capture(16m).WithGratuity(2m).Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }
        
       
        [TestMethod]
        public void CreditAuthorizationWithCommercialRequest()
        {
            var response = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var capture = response.Capture(16m).WithGratuity(2m).Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }
        [TestMethod]
        public void CreditSaleWithCommercialRequest()
        {
            string clientTxnId = new Random().Next(100000000, 999999999).ToString();

            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithClientTransactionId(clientTxnId)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(clientTxnId, response.ClientTransactionId);
        }

        [TestMethod]
        public void CreditAuthWithConvenienceAmt() {
            var response = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithConvenienceAmount(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditAuthWithShippingAmt() {
            var response = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithShippingAmt(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ShippingAmount);
        }

        [TestMethod]
        public void CreditAuthorizationWithCOF()
        {
            Transaction response = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithCardBrandStorage(StoredCredentialInitiator.Merchant)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.CardBrandTransactionId);

            Transaction cofResponse = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithCardBrandStorage(StoredCredentialInitiator.CardHolder, response.CardBrandTransactionId, "07")
                .Execute();
            Assert.IsNotNull(cofResponse);
            Assert.AreEqual("00", cofResponse.ResponseCode);

            Transaction capture = cofResponse.Capture(16m)
                .WithGratuity(2m)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void CreditSale() {
            string clientTxnId = new Random().Next(100000000, 999999999).ToString();

            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithClientTransactionId(clientTxnId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(clientTxnId, response.ClientTransactionId);
        }

        [TestMethod]
        public void CreditSaleWithRefund() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var trans = Transaction.FromId(response.TransactionId)
                .Refund(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(trans);
            Assert.AreEqual("00", trans.ResponseCode);
        }
        [TestMethod]
        public void CreditSale_WithFallbackData()
        {
            track.PinBlock = "abcjhvcjbvhjxbvjxh";
            var response = track.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithEmvFallbackData(EmvFallbackCondition.ChipReadFailure,EmvLastChipRead.Successful)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        [TestMethod]
        public void CreditSale_WithTagData()
        {
            track.PinBlock = "abcjhvcjbvhjxbvjxh";
            var tagData = "9F1A0208409C0150950500000088009F0702FF009F03060000000000009F2701809F3901059F0D05B850AC88009F350121500B56697361204372656469745F3401019F0802008C9F120B56697361204372656469749F0E0500000000009F360200759F40057E0000A0019F0902008C9F0F05B870BC98009F370425D254AC5F280208409F33036028C882023C004F07A00000000310109F4104000000899F0607A00000000310105F2A0208409A031911229F02060000000001009F2608D4EC434B9C1CBB358407A00000000310109F100706010A03A088069B02E8009F34031E0300";
            var response = track.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithTagData(tagData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        [TestMethod]
        public void CreditTrackData_SaleSwipe_Chip()
        {
            CreditTrackData creditTrackData = new CreditTrackData()
            {
                Value = "<E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|>;",
                PinBlock = track.PinBlock = "abcjhvcjbvhjxbvjxh",
                EntryMethod = EntryMethod.Swipe
            };

            //string tagData = "Any Emv Tags";

            decimal amount = 25m;

            var response = creditTrackData.Charge()
                .WithCurrency("USD")
                .WithAmount(amount)
                .WithAllowDuplicates(true)
                //.WithTagData(tagData)
                .Execute();
        }


        [TestMethod]
        public void CreditSaleWithCOF() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithCardBrandStorage(StoredCredentialInitiator.CardHolder)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.CardBrandTransactionId);

            Transaction cofResponse = card.Charge(15m)
                .WithCurrency("USD")
                .WithCardBrandStorage(StoredCredentialInitiator.Merchant, response.CardBrandTransactionId)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(cofResponse);
            Assert.AreEqual("00", cofResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditVerifyWithCOF() {
            Transaction response = card.Verify()
                .WithAllowDuplicates(true)
                .WithCardBrandStorage(StoredCredentialInitiator.CardHolder)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.CardBrandTransactionId);

            Transaction cofResponse = card.Verify()
                .WithAllowDuplicates(true)
                .WithCardBrandStorage(StoredCredentialInitiator.Merchant, response.CardBrandTransactionId)
                .Execute();
            Assert.IsNotNull(cofResponse);
            Assert.AreEqual("00", cofResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditSaleWithConvenienceAmt() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithConvenienceAmount(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditSaleWithShippingAmt() {
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithShippingAmt(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ShippingAmount);
        }

        [TestMethod]
        public void CreditSaleWithSurchargeAmount() {
            var amount = 10m;
            var response = card.Charge(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithSurchargeAmount(amount * .35m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(amount * .35m, report.SurchargeAmount);
        }

        [TestMethod]
        public void CreditSaleAndEditWithSurchargeAmount() {
            var amount = 10m;
            var response = card.Charge(amount)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var edit = response.Edit()
                .WithSurchargeAmount(amount * .35m)
                .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(amount * .35m, report.SurchargeAmount);
        }

        [TestMethod]
        public void CreditOfflineAuth() {
            var response = card.Authorize(16m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditOfflineAuthWithConvenienceAmt() {
            var response = card.Authorize(16m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .WithConvenienceAmount(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditOfflineWithShippingAmt() {
            var response = card.Authorize(16m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .WithShippingAmt(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ShippingAmount);
        }

        [TestMethod]
        public void CreditOfflineSale() {
            var response = card.Charge(17m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditOfflineSaleWithConvenienceAmt() {
            var response = card.Charge(17m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .WithConvenienceAmount(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ConvenienceAmount);
        }

        [TestMethod]
        public void CreditOfflineSaleWithShippingAmt() {
            var response = card.Charge(17m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .WithShippingAmt(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            TransactionSummary report = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(report);
            Assert.AreEqual(2m, report.ShippingAmount);
        }

        [TestMethod]
        public void CreditRefund() {
            var response = card.Refund(16m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditReverse() {
            var response = card.Reverse(15m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVerify() {
            var response = card.Verify()
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSwipeAuthorization() {
            var response = track.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var capture = response.Capture(16m).WithGratuity(2m).Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void CreditSwipeSale() {
            var response = track.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSwipeOfflineAuth() {
            var response = track.Authorize(16m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSwipeOfflineSale() {
            var response = card.Charge(17m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("12345")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void CreditSwipeAddValue() {
            var response = track.AddValue(16m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSwipeBalanceInquiry() {
            var response = track.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void CreditSwipeRefund() {
            var response = card.Refund(16m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditSwipeReverse() {
            var response = track.Charge(19m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var reverseResponse = track.Reverse(19m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditSwipeVerify() {
            var response = card.Verify()
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoidFromTransactionId() {
            var response = card.Authorize(10.00m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var voidResponse = Transaction.FromId(response.TransactionId)
                .Void()
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void CreditTestWithNewCryptoURL() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Authorize(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditTestWithClientTransactionId() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .WithClientTransactionId("1002755")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CreditTokenize() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            }, "tokenize");

            var token = card.Tokenize("tokenize");
            Assert.IsNotNull(token);
        }

        // Purpose of this test is to verify and demonstrate handling of 
        // EMVChipCondition Element with Credit transactions
        [TestMethod]
        public void EmvChipConditionTesting()
        {
            track.EntryMethod = EntryMethod.Swipe;

            // send emvChipConditionType = CHIP_FAILED_PREV_SUCCESS
            var response = track.Charge(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithEmvFallbackData(
                    EmvFallbackCondition.ChipReadFailure,
                    EmvLastChipRead.Successful
                )
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var responseReport = ReportingService.FindTransactions()
                .WithTransactionId(response.TransactionId)
                .Execute()[0];

            // No idea why, but at least in the cert gateway a "1" property
            // value here means "CHIP_FAILED_PREV_SUCCESS" for this report type
            Assert.AreEqual("1", responseReport.EmvChipCondition);

            // send emvChipConditionType = CHIP_FAILED_PREV_FAILED
            var responseDos = track.Charge(14m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithEmvFallbackData(
                    EmvFallbackCondition.ChipReadFailure,
                    EmvLastChipRead.Failed
                )
                .Execute();

            Assert.IsNotNull(responseDos);
            Assert.AreEqual("00", responseDos.ResponseCode);

            var responseDosReport = ReportingService.FindTransactions().WithTransactionId(responseDos.TransactionId).Execute()[0];

            // Same as above, at least in the cert gateway a "2" property
            // value here means "CHIP_FAILED_PREV_FAILED" for this report type
            Assert.AreEqual("2", responseDosReport.EmvChipCondition);
        }
    }
}
