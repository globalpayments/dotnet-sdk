using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoAutoSubTests {
        private CreditCardData card;
        private CreditTrackData track;

        public PorticoAutoSubTests() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });

            card = new CreditCardData {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            track = new CreditTrackData {
                Value = "<E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|>;",
                EncryptionData = new EncryptionData {
                    Version = "01"
                }
            };
        }

        [TestMethod]
        public void TotalAmountTest() {
            var autoSub = new AutoSubstantiation {
                ClinicSubTotal = 25m,
                VisionSubTotal = 25m,
                DentalSubTotal = 25m,
                PrescriptionSubTotal = 25m
            };
            Assert.AreEqual(100m, autoSub.TotalHealthcareAmount);
        }

        [TestMethod]
        public void Dental() {
            var autoSub = new AutoSubstantiation {
                MerchantVerificationValue = "12345",
                RealTimeSubstantiation = false,
                DentalSubTotal = 150m
            };

            var response = card.Charge(215m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Vision() {
            var autoSub = new AutoSubstantiation {
                MerchantVerificationValue = "12345",
                RealTimeSubstantiation = false,
                VisionSubTotal = 150m
            };

            var response = track.Charge(215m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ClinicOrOther() {
            var autoSub = new AutoSubstantiation {
                MerchantVerificationValue = "12345",
                RealTimeSubstantiation = false,
                ClinicSubTotal = 150m
            };

            var response = card.Charge(215m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Prescription() {
            var autoSub = new AutoSubstantiation {
                MerchantVerificationValue = "12345",
                RealTimeSubstantiation = false,
                PrescriptionSubTotal = 150m
            };

            var response = track.Charge(215m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(BuilderException))]
        public void AllSubTotals() {
            var autoSub = new AutoSubstantiation {
                MerchantVerificationValue = "12345",
                RealTimeSubstantiation = false,
                ClinicSubTotal = 25m,
                VisionSubTotal = 25m,
                DentalSubTotal = 25m,
                PrescriptionSubTotal = 25m
            };

            var response = card.Charge(215m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute();
        }

        [TestMethod]
        public void ThreeSubTotals() {
            var autoSub = new AutoSubstantiation {
                MerchantVerificationValue = "12345",
                RealTimeSubstantiation = false,
                ClinicSubTotal = 25m,
                VisionSubTotal = 25m,
                DentalSubTotal = 25m
            };

            var response = track.Charge(215m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void TwoSubTotals() {
            var autoSub = new AutoSubstantiation {
                MerchantVerificationValue = "12345",
                RealTimeSubstantiation = false,
                ClinicSubTotal = 25m,
                VisionSubTotal = 25m
            };

            var response = card.Charge(215m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
