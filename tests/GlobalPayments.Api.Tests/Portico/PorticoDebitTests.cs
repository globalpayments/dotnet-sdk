using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests
{
    [TestClass]
    public class PorticoDebitTests
    {
        DebitTrackData track;

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw",
                IsSafDataSupported = true
            });

            track = new DebitTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EncryptionData = new EncryptionData {
                    Version = "01"
                }
            };
        }

        [TestMethod]
        public void DebitSale() {
            var response = track.Charge(17.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod, Ignore]
        public void DebitAddValue() {
            var response = track.AddValue(15.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void DebitRefund() {
            var response = track.Refund(16.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void DebitReverse() {
            var response = track.Reverse(17.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod, ExpectedException(typeof(UnsupportedTransactionException))]
        public void DebitRefundFromTransactionId() {
            Transaction.FromId("1234567890", PaymentMethodType.Debit).Refund().Execute();
        }

        [TestMethod]
        public void DebitReverseFromTransactionId()
        {
            var response = track.Charge(17.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();

            var reversalResponse = Transaction.FromId(response.TransactionId, PaymentMethodType.Debit).Reverse(17.01m).Execute();
            Assert.AreEqual("00", reversalResponse.ResponseCode, reversalResponse.ResponseMessage);
        }

        [TestMethod]
        public void debitSaleWithNewCryptoURL() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            });

            track = new DebitTrackData
            {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                PinBlock = "32539F50C245A6A93D123412324000AA",
                EncryptionData = new EncryptionData
                {
                    Version = "01"
                }
            };
            var response = track.Charge(17.01m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        // Purpose of this test is to verify and demonstrate handling of 
        // EMVChipCondition Element with Debit transactions
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
