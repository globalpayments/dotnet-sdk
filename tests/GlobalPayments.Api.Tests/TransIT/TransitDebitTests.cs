using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.TransIT {
    [TestClass]
    public class TransitDebitTests {
        private DebitTrackData track;

        public TransitDebitTests() {
            var acceptorConfig = new AcceptorConfig {
                /* The following are the default values for the AcceptorConfig */

                //CardDataInputCapability = CardDataInputCapability.MagStripe_KeyEntry,
                //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Attended,
                //CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None,
                //TerminalOutputCapability = TerminalOutputCapability.Unknown,
                //PinCaptureCapability = PinCaptureCapability.Unknown,
                //CardCaptureCapability = false,
                //CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated,
                //CardDataOutputCapability = CardDataOutputCapability.None
            };

            //ServicesContainer.ConfigureService(new TransitConfig {
            //    AcceptorConfig = acceptorConfig,
            //    DeveloperId = "003226G001",
            //    DeviceId = "88700000322601",
            //    MerchantId = "887000003226",
            //    TransactionKey = "H1O8QTS2JVA9OFMQ3FGEH6D5E28X1KS9"
            //});

            track = new DebitTrackData {
                Value = "4355567063338=2012101HJNw/ewskBgnZqkL",
                PinBlock = "62968D2481D231E1A504010024A00014",
                EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gcOTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0m+/d4SO9TEshhRGUUQzVBrBvP/Os1qFx+6zdQp1ejjUCoDmzoUMbil9UG73zBxxTOy25f3Px0p8joyCh8PEWhADz1BkROJT3q6JnocQE49yYBHuFK0obm5kqUcYPfTY09vPOpmN+wp45gJY9PhkJF5XvPsMlcxX4/JhtCshegz4AYrcU/sFnI+nDwhy295BdOkVN1rn00jwCbRcE900kj3UsFfyc", "2")
            };
        }

        [TestMethod]
        public void Sale_Swipe() {
            var response = track.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }
    }
}
