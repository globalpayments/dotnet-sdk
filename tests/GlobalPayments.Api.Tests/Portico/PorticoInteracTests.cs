using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass, Ignore]
    public class PorticoInteracTests {
        DebitTrackData track;
        string tagData;

        [TestInitialize]
        public void Init() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                LicenseId = 124964,
                SiteId = 124992,
                DeviceId = 145,
                Username = "9158402",
                Password = "$Test1234"
            });

            track = new DebitTrackData {
                Value = ";4024720012345671=18125025432198712345?",
                EntryMethod = EntryMethod.Proximity,
                PinBlock = "AFEC374574FC90623D010000116001EE"
            };

            tagData = "82021C008407A0000002771010950580000000009A031709289C01005F280201245F2A0201245F3401019F02060000000010009F03060000000000009F080200019F090200019F100706010A03A420009F1A0201249F26089CC473F4A4CE18D39F2701809F3303E0F8C89F34030100029F3501229F360200639F370435EFED379F410400000019";
        }

        [TestMethod]
        public void DebitInteracPosNumber() {
            var response = track.Charge(8m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithPosSequenceNumber("000010010770")
                .WithTagData(tagData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void DebitInteracAccountTypeChecking() {
            var response = track.Charge(8m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAccountType(AccountType.CHECKING)
                .WithTagData(tagData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void DebitInteracAccountTypeSavings() {
            var response = track.Charge(8m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAccountType(AccountType.SAVINGS)
                .WithTagData(tagData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void DebitInteracMessageAuthenticationCode() {
            var response = track.Charge(8m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithMessageAuthenticationCode("AFEC374")
                .WithTagData(tagData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void DebitInteracChipConditionFailed() {
            var response = track.Charge(8m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithEmvFallbackData(EmvFallbackCondition.ChipReadFailure, EmvLastChipRead.Successful)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void DebitInteracChipConditionFailedTwice() {
            var response = track.Charge(8m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithEmvFallbackData(EmvFallbackCondition.ChipReadFailure, EmvLastChipRead.Failed)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void DebitInteracChipConditionWithTagData() {
            track.Charge(8m)
                .WithCurrency("USD")
                .WithEmvFallbackData(EmvFallbackCondition.ChipReadFailure, EmvLastChipRead.Successful)
                .WithTagData(tagData)
                .Execute();
        }

        [TestMethod]
        public void DebitInteracResponseFields() {
            var response = track.Charge(8m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithPosSequenceNumber("1")
                .WithAccountType(AccountType.CHECKING)
                .WithMessageAuthenticationCode("AFEC374")
                .WithTagData(tagData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
            Assert.IsNotNull(response.EmvIssuerResponse, "EmvIsserResponse was null");
            Assert.IsNotNull(response.DebitMac, "DebitMac was null");
            Assert.IsNotNull(response.HostResponseDate, "HostResponseDate was null");
        }
    }
}
