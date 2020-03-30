using System;
using System.Diagnostics;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Certifications {
    [TestClass]
    public class LodgingCertification {
        private CreditCardData card;
        private CreditTrackData track;

        public LodgingCertification() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SiteId = 144101,
                LicenseId = 144012,
                DeviceId = 6402470,
                Username = "701395572",
                Password = "$Test1234",
                ServiceUrl = "https://cert.api2-c.heartlandportico.com",
                DeveloperId = "002914",
                VersionNumber = "4268"
            });
        }

        [TestMethod]
        public void Lodging_000_CloseBatch() {
            try {
                var response = BatchService.CloseBatch();
                Assert.IsNotNull(response);
                Debug.WriteLine(string.Format("Batch ID: {0}", response.Id));
                Debug.WriteLine(string.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (GatewayException exc) {
                if (exc.ResponseMessage != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }

        /*
            Check In/Check Out (Single Stay) - SALE : SWIPED
        */
        [TestMethod]
        public void Lodging_001_SaleVisaSwiped_SingleStay() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_002a_SaleMasterSwiped_SingleStay() {
            track = TestCards.MasterCardSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Charge(11m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_002b_SaleMaster24Swiped_SingleStay() {
            track = TestCards.MasterCard24Swipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Charge(11.50m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_003_SaleDiscoverSwiped_SingleStay() {
            track = TestCards.DiscoverSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Charge(12m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_004_SaleAmexSwiped_SingleStay() {
            track = TestCards.AmexSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Charge(13m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_005_SaleJcbSwiped_SingleStay() {
            track = TestCards.JcbSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Charge(14m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            Check In/Check Out (Single Stay) - SALE : KEYED, CARD PRESENT
        */
        [TestMethod]
        public void Lodging_006_SaleVisaKeyed_CardPresent_SingleStay() {
            card = TestCards.VisaManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(15m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_007a_SaleMasterKeyed_CardPresent_SingleStay() {
            card = TestCards.MasterCardManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(16m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_007b_SaleMaster2SeriesKeyed_CardPresent_SingleStay() {
            card = TestCards.MasterCardSeries2Manual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(16.50m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_008_SaleDiscoverManualKeyed_CardPresent_SingleStay() {
            card = TestCards.DiscoverManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(17m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_009_SaleAmexManualKeyed_CardPresent_SingleStay() {
            card = TestCards.AmexManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(18m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_010_SaleJcbManualKeyed_CardPresent_SingleStay() {
            card = TestCards.JcbManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(19m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            Check In/Check Out (Single Stay) - SALE : KEYED, CARD NOT PRESENT
        */
        [TestMethod]
        public void Lodging_011_SaleVisaManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.VisaManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(20m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_012a_SaleMasterManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.MasterCardManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(21m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_012b_SaleMaster2SeriesManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.MasterCardSeries2Manual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(21.50m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_013_SaleDiscoverManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.DiscoverManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(22m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_014_SaleAmexManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.AmexManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(23m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_015_SaleJcbManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.JcbManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Charge(24m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            Contactless
            Check In/Check Out - AUTHORIZATIONS : SWIPED
        */
        [TestMethod]
        public void Lodging_016_AuthVisaSwiped_SingleStay() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Authorize(25m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // capture
            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_017a_AuthMasterSwiped_SingleStay() {
            track = TestCards.MasterCardSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Authorize(26m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_017b_AuthMaster24Swiped_SingleStay() {
            track = TestCards.MasterCard24Swipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Authorize(26.50m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_018_AuthDiscoverSwiped_SingleStay() {
            track = TestCards.DiscoverSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Authorize(27m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_019_AuthAmexSwiped_SingleStay() {
            track = TestCards.AmexSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Authorize(28m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_020_AuthJcbSwiped_SingleStay() {
            track = TestCards.JcbSwipe();

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = track.Authorize(29m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /*
            Check In/Check Out - AUTHORIZATIONS : KEYED, CARD PRESENT
        */
        [TestMethod]
        public void Lodging_021_AuthVisaKeyed_CardPresent_SingleStay() {
            card = TestCards.VisaManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(30m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_022_AuthMasterKeyed_CardPresent_SingleStay() {
            card = TestCards.MasterCardManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(31m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_023_AuthDiscoverKeyed_CardPresent_SingleStay() {
            card = TestCards.DiscoverManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(32m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_024_AuthAmexKeyed_CardPresent_SingleStay() {
            card = TestCards.AmexManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(33m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_025_AuthJcbKeyed_CardPresent_SingleStay() {
            card = TestCards.JcbManual(true, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(34m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /*
            Check In/Check Out - AUTHORIZATIONS : KEYED, CARD NOT PRESENT
        */
        [TestMethod]
        public void Lodging_026_AuthVisaManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.VisaManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(35m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_027_AuthMasterManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.MasterCardManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(36m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_028_AuthDiscoverManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.DiscoverManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(37m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_029_AuthAmexManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.AmexManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(38m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_030_AuthJcbManualKeyed_CardNotPresent_SingleStay() {
            card = TestCards.JcbManual(false, true);

            var lodgingData = new LodgingData { StayDuration = 1 };

            Transaction response = card.Authorize(39m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /*
            Advanced Deposit - SALES
        */
        [TestMethod]
        public void Lodging_031_SaleVisaManualKeyed_AdvancedDeposit() {
            card = TestCards.VisaManual(false, true);

            var lodgingData = new LodgingData {
                    StayDuration = 1,
                    AdvancedDepositType = AdvancedDepositType.CARD_DEPOSIT
            };

            Transaction response = card.Charge(41m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_032_SaleAmexManualKeyed_AdvancedDeposit() {
            card = TestCards.AmexManual(false, true);

            var lodgingData = new LodgingData {
                    StayDuration = 2,
                    AdvancedDepositType = AdvancedDepositType.CARD_DEPOSIT
            };

            Transaction response = card.Charge(80m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            No Show - SALES
        */
        [TestMethod]
        public void Lodging_033_SaleMasterManualKeyed_NoShow() {
            card = TestCards.MasterCardManual(false, true);

            var lodgingData = new LodgingData {
                StayDuration = 1,
                NoShow = true
            };

            Transaction response = card.Charge(42m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_034_SaleAmexManualKeyed_NoShow() {
            card = TestCards.AmexManual(false, true);

            var lodgingData = new LodgingData {
                StayDuration = 1,
                AdvancedDepositType = AdvancedDepositType.ASSURED_RESERVATION,
                NoShow = true
            };

            Transaction response = card.Charge(43m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            Visa Prestigious Property - SALES
        */
        [TestMethod]
        public void Lodging_035_SaleVisaSwiped_PrestigiousProperty() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData {
                StayDuration = 1,
                PrestigiousPropertyLimit = PrestigiousPropertyLimit.LIMIT_500
            };

            Transaction response = track.Charge(44m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_036_SaleVisaSwiped_PrestigiousProperty() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData {
                StayDuration = 1,
                PrestigiousPropertyLimit = PrestigiousPropertyLimit.LIMIT_1000
            };

            Transaction response = track.Charge(45m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_037_SaleVisaSwiped_PrestigiousProperty() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData {
                StayDuration = 1,
                PrestigiousPropertyLimit = PrestigiousPropertyLimit.LIMIT_1500
            };

            Transaction response = track.Charge(46m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            Visa Prestigious Property - AUTHORIZATIONS
        */
        [TestMethod]
        public void Lodging_038_AuthVisaSwiped_PrestigiousProperty() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData {
                StayDuration = 1,
                PrestigiousPropertyLimit = PrestigiousPropertyLimit.LIMIT_500
            };

            Transaction response = track.Authorize(44m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_039_AuthVisaSwiped_PrestigiousProperty() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData {
                StayDuration = 1,
                PrestigiousPropertyLimit = PrestigiousPropertyLimit.LIMIT_1000
            };

            Transaction response = track.Authorize(45m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_040_AuthVisaSwiped_PrestigiousProperty() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData {
                StayDuration = 1,
                PrestigiousPropertyLimit = PrestigiousPropertyLimit.LIMIT_1500
            };

            Transaction response = track.Authorize(46m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /*
            MasterCard Preferred Customer - SALES
        */
        [TestMethod]
        public void Lodging_041_SaleMasterSwiped_PreferredCustomer() {
            track = TestCards.MasterCardSwipe();

            var lodgingData = new LodgingData {
                StayDuration = 1,
                PreferredCustomer = true
            };

            Transaction response = track.Charge(47m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_042_SaleMasterManualKeyed_PreferredCustomer() {
            card = TestCards.MasterCardManual(false, true);

            var lodgingData = new LodgingData {
                StayDuration = 2,
                PreferredCustomer = true
            };

            Transaction response = card.Charge(48m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            MasterCard Preferred Customer - AUTHORIZATIONS
        */
        [TestMethod]
        public void Lodging_043_AuthMasterSwiped_PreferredCustomer() {
            track = TestCards.MasterCardSwipe();

            var lodgingData = new LodgingData {
                StayDuration = 1,
                PreferredCustomer = true
            };

            Transaction response = track.Authorize(47m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_044_AuthMasterManualKeyed_PreferredCustomer() {
            card = TestCards.MasterCardManual(false, true);

            var lodgingData = new LodgingData {
                StayDuration = 2,
                PreferredCustomer = true
            };

            Transaction response = card.Authorize(48m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /*
            Additional / Extra Charges - SALES
        */
        [TestMethod]
        public void Lodging_045_SaleVisaSwiped_ExtraCharges() {
            track = TestCards.VisaSwipe();

            var lodgingData = new LodgingData()
                    .AddExtraCharge(ExtraChargeType.Restaurant);

            Transaction response = track.Charge(49m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_045a_SaleVisaSwiped_ExtraChargesEdit() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Charge(49m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var lodgingData = new LodgingData()
                    .AddExtraCharge(ExtraChargeType.Restaurant);

            Transaction editResponse = response.Edit()
                    .WithAmount(49m)
                    .WithCurrency("USD")
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        [TestMethod]
        public void Lodging_046_SaleMasterSwiped_ExtraCharges() {
            track = TestCards.MasterCardSwipe();

            var lodgingData = new LodgingData()
                    .AddExtraCharge(ExtraChargeType.GiftShop)
                    .AddExtraCharge(ExtraChargeType.MiniBar)
                    .AddExtraCharge(ExtraChargeType.Telephone);

            Transaction response = track.Charge(50m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_046a_SaleMasterSwiped_ExtraChargesEdit() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Charge(50m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var lodgingData = new LodgingData()
                    .AddExtraCharge(ExtraChargeType.GiftShop)
                    .AddExtraCharge(ExtraChargeType.MiniBar)
                    .AddExtraCharge(ExtraChargeType.Telephone);

            Transaction editResponse = response.Edit()
                    .WithAmount(50m)
                    .WithCurrency("USD")
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        [TestMethod]
        public void Lodging_047_SaleDiscoverSwiped_ExtraCharges() {
            track = TestCards.DiscoverSwipe();

            var lodgingData = new LodgingData()
                    .AddExtraCharge(ExtraChargeType.Laundry);

            Transaction response = track.Charge(51m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_047a_SaleDiscoverSwiped_ExtraChargesEdit() {
            track = TestCards.DiscoverSwipe();

            Transaction response = track.Charge(51m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var lodgingData = new LodgingData()
                    .AddExtraCharge(ExtraChargeType.Laundry);

            Transaction editResponse = response.Edit()
                    .WithAmount(50m)
                    .WithCurrency("USD")
                    .WithLodgingData(lodgingData)
                    .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        /*
            Additional / Extra Charges - AUTHORIZATIONS
        */
        [TestMethod]
        public void Lodging_048_AuthVisaSwiped_ExtraCharges() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Authorize(49m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var lodgingData = new LodgingData()
                   .AddExtraCharge(ExtraChargeType.Restaurant);

            Transaction captureResponse = response.Capture()
                   .WithLodgingData(lodgingData)
                   .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Lodging_049_AuthMasterSwiped_ExtraCharges() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Authorize(50m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var lodgingData = new LodgingData()
                    .AddExtraCharge(ExtraChargeType.GiftShop)
                    .AddExtraCharge(ExtraChargeType.MiniBar)
                    .AddExtraCharge(ExtraChargeType.Telephone);

            Transaction captureResponse = response.Capture()
                   .WithLodgingData(lodgingData)
                   .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Lodging_050_AuthDiscoverSwiped_ExtraCharges() {
            track = TestCards.DiscoverSwipe();

            Transaction response = track.Authorize(51m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var lodgingData = new LodgingData()
                   .AddExtraCharge(ExtraChargeType.Laundry);

            Transaction captureResponse = response.Capture()
                   .WithLodgingData(lodgingData)
                   .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        /*
            Partial Approvals - SALES
        */
        [TestMethod]
        public void Lodging_051_SaleMasterSwiped_PartialApproval() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Charge(130m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(110m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Lodging_052_SaleDiscoverManualKeyed_PartialApproval() {
            card = TestCards.DiscoverManual(true, true);

            Transaction response = card.Charge(145m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(65m, response.AuthorizedAmount);
        }

        /*
            Partial Approvals - AUTHORIZATIONS
        */
        [TestMethod]
        public void Lodging_053_AuthMasterSwiped_PartialApproval() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Authorize(130m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(110m, response.AuthorizedAmount);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_054_AuthDiscoverManualKeyed_PartialApproval() {
            card = TestCards.DiscoverManual(true, true);

            Transaction response = card.Authorize(145m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(65m, response.AuthorizedAmount);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /*
            CARD VERIFY
        */
        [TestMethod]
        public void Lodging_055_VerifyVisaSwiped() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Verify()
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_056_VerifyMasterManualKeyed() {
            card = TestCards.MasterCardManual(false, true);

            Transaction response = card.Verify()
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_057_VerifyDiscoverSwiped() {
            track = TestCards.DiscoverSwipe();

            Transaction response = track.Verify()
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            FORCE / VOICE AUTHORIZATIONS
        */
        [TestMethod]
        public void Lodging_058_OfflineAuth() {
            card = TestCards.VisaManual(true, true);

            Transaction response = card.Authorize(52m)
                    .WithCurrency("USD")
                    .WithOfflineAuthCode("654321")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            RETURN
        */
        [TestMethod]
        public void Lodging_059_ReturnByCard() {
            card = TestCards.DiscoverManual(false, true);

            Transaction response = card.Refund(53m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            RETURN by TxnID
        */
        [TestMethod]
        public void Lodging_059a_ReturnByTxnId_Visa() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Charge(53m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction refund = response.Refund(53m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(refund);
            Assert.AreEqual("00", refund.ResponseCode);
        }

        [TestMethod]
        public void Lodging_059b_ReturnByTxnId_MasterCard() {
            card = TestCards.MasterCardManual(true, true);

            Transaction response = card.Charge(54m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction refund = response.Refund(54m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(refund);
            Assert.AreEqual("00", refund.ResponseCode);
        }

        /*
            LEVEL II Corporate Purchase Card - SALES
        */
        [TestMethod]
        public void Lodging_060_LevelII_Sale_Visa_TaxNotUsed() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Charge(112.34m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("B", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_061_LevelII_Sale_Visa_SalesTax() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Charge(112.34m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("R", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_062_LevelII_Sale_Visa_TaxExempt() {
            card = TestCards.VisaManual(true, true);

            Transaction response = card.Charge(123.45m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("R", response.CommercialIndicator);

            Transaction edit = response.Edit()
                    .WithCommercialData(new CommercialData(TaxType.TAXEXEMPT))
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_063_LevelII_Sale_VisaManual_SalesTax() {
            card = TestCards.VisaManual(true, true);

            Transaction response = card.Charge(134.56m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m,
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_064_LevelII_Sale_Master_TaxNotUsed() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Charge(111.06m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_065_LevelII_Sale_Master_SalesTax() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Charge(111.07m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_066_LevelII_Sale_MasterManual_SalesTax() {
            card = TestCards.MasterCardManual(true, true);

            Transaction response = card.Charge(111.08m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m,
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_067_LevelII_Sale_MasterManual_TaxExempt() {
            card = TestCards.MasterCardManual(true, true);

            Transaction response = card.Charge(111.09m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.TAXEXEMPT) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_068_LevelII_Sale_Amex_SalesTax() {
            track = TestCards.AmexSwipe();

            Transaction response = track.Charge(111.10m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m,
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_069_LevelII_Sale() {
            track = TestCards.AmexSwipe();

            Transaction response = track.Charge(111.11m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_070_LevelII_Sale_Amex_TaxNotUsed() {
            card = TestCards.AmexManual(true, true);

            Transaction response = card.Charge(111.12m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void Lodging_071_LevelII_Sale_AmexManual_TaxExempt() {
            card = TestCards.AmexManual(true, true);

            Transaction response = card.Charge(111.13m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.TAXEXEMPT) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        /*
            LEVEL II Corporate Purchase Card - AUTHORIZATIONS
        */
        [TestMethod]
        public void Lodging_072_LevelII_Authorization_Visa_TaxNotUsed() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Authorize(112.34m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("B", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_073_LevelII_Authorization_Visa_SalesTax() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Authorize(112.34m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("B", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_074_LevelII_Authorization_VisaManual_TaxExempt() {
            card = TestCards.VisaManual(true, true);

            Transaction response = card.Authorize(123.45m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("R", response.CommercialIndicator);

            Transaction edit = response.Edit()
                    .WithCommercialData(new CommercialData(TaxType.TAXEXEMPT))
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_075_LevelII_Authorization_VisaManual_SalesTax() {
            card = TestCards.VisaManual(true, true);

            Transaction response = card.Authorize(134.56m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m,
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_076_LevelII_Authorization_Master_TaxNotUsed() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Authorize(111.06m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_077_LevelII_Authorization_Master_SalesTax() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Authorize(111.07m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_078_LevelII_Authorization_MasterManual_SalesTax() {
            card = TestCards.MasterCardManual(true, true);

            Transaction response = card.Authorize(111.08m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m,
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_079_LevelII_Authorization_MasterManual_TaxExempt() {
            card = TestCards.MasterCardManual(true, true);

            Transaction response = card.Authorize(111.09m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.TAXEXEMPT) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_080_LevelII_Authorization_Amex_SalesTax() {
            track = TestCards.AmexSwipe();

            Transaction response = track.Authorize(111.10m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m,
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_081_LevelII_Authorization() {
            track = TestCards.AmexSwipe();

            Transaction response = track.Authorize(111.11m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 1m
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_082_LevelII_Authorization_AmexManual_TaxNotUsed() {
            card = TestCards.AmexManual(true, true);

            Transaction response = card.Authorize(111.12m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_083_LevelII_Authorization_AmexManual_TaxExempt() {
            card = TestCards.AmexManual(true, true);

            Transaction response = card.Authorize(111.13m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.TAXEXEMPT) {
                PoNumber = "9876543210"
            };

            Transaction edit = response.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /*
            Incremental Authorizations - SALES
        */
        [TestMethod]
        public void Lodging_084_IncrementalAuth_VisaSale() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Charge(115m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(23m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);
        }

        [TestMethod]
        public void Lodging_085_IncrementalAuth_MasterSale() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Charge(116m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(24m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);
        }

        /*
            Incremental Authorizations - AUTHORIZATIONS
        */
        [TestMethod]
        public void Lodging_086_IncrementalAuth_VisaAuth() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Authorize(115m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(23m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod]
        public void Lodging_087_IncrementalAuth_MasterAuth() {
            track = TestCards.MasterCardSwipe();

            Transaction response = track.Authorize(116m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction incremental = response.Increment(24m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(incremental);
            Assert.AreEqual("00", incremental.ResponseCode);

            Transaction capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        /*
            ONLINE VOID / REVERSAL (Required)
        */
        [TestMethod]
        public void Lodging_088_OnlineVoid_Visa() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Charge(122m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reversal = response.Void().Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Lodging_089_OnlineVoid_Master() {
            card = TestCards.MasterCardManual(true, true);

            Transaction response = card.Charge(124m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reversal = response.Void().Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Lodging_090_OnlineVoid_DiscoverManual() {
            card = TestCards.DiscoverManual(false, true);

            Transaction response = card.Charge(125m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reversal = response.Void().Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Lodging_091_OnlineVoid_Discover() {
            track = TestCards.DiscoverSwipe();

            Transaction response = track.Charge(155m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(100m, response.AuthorizedAmount);

            Transaction reversal = response.Reverse(100m).Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        /*
            ONLINE VOID / REVERSAL FOR INCREMENTALS
        */
        [TestMethod]
        public void Lodging_092_IncrementalReversal_Visa() {
            track = TestCards.VisaSwipe();

            Transaction response = track.Charge(126m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction firstIncrement = response.Increment(26m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(firstIncrement);
            Assert.AreEqual("00", firstIncrement.ResponseCode);

            Transaction secondIncrement = response.Increment(31m)
                   .WithCurrency("USD")
                   .Execute();
            Assert.IsNotNull(secondIncrement);
            Assert.AreEqual("00", secondIncrement.ResponseCode);

            Transaction reversal = response.Reverse(126m).Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Lodging_093_IncrementalReversal_Discover() {
            track = TestCards.DiscoverSwipe();

            Transaction response = track.Charge(127m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction firstIncrement = response.Increment(27m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(firstIncrement);
            Assert.AreEqual("00", firstIncrement.ResponseCode);

            Transaction secondIncrement = response.Increment(32m)
                   .WithCurrency("USD")
                   .Execute();
            Assert.IsNotNull(secondIncrement);
            Assert.AreEqual("00", secondIncrement.ResponseCode);

            Transaction reversal = response.Reverse(127m).Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        /*
            PIN DEBIT CARD FUNCTIONS - SALE
        */
        [TestMethod]
        public void Lodging_094_DebitSale_Visa() {
            DebitTrackData track = TestCards.AsDebit(TestCards.VisaSwipe(), "32539F50C245A6A93D123412324000AA");

            Transaction response = track.Charge(139m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_095_DebitSale_Master() {
            DebitTrackData track = TestCards.AsDebit(TestCards.MasterCardSwipe(), "F505AD81659AA42A3D123412324000AB");

            Transaction response = track.Charge(135m)
                    .WithCurrency("USD")
                    .WithCashBack(5m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            PARTIALLY - APPROVED PURCHASE
        */
        [TestMethod]
        public void Lodging_096_DebitPartialApproval() {
            DebitTrackData track = TestCards.AsDebit(TestCards.MasterCardSwipe(), "F505AD81659AA42A3D123412324000AB");

            Transaction response = track.Charge(33m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(22m, response.AuthorizedAmount); ;
        }

        [TestMethod]
        public void Lodging_096a_DebitPartialApproval() {
            DebitTrackData track = TestCards.AsDebit(TestCards.VisaSwipe(), "32539F50C245A6A93D123412324000AA");

            Transaction response = track.Charge(44m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(33m, response.AuthorizedAmount);
        }
        /*
            RETURN
        */
        [TestMethod]
        public void Lodging_097_DebitReturn() {
            DebitTrackData track = TestCards.AsDebit(TestCards.VisaSwipe(), "32539F50C245A6A93D123412324000AA");

            Transaction response = track.Refund(40m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            ONLINE VOID / REVERSAL (Required)
        */
        [TestMethod]
        public void Lodging_098_DebitReversal_Visa() {
            DebitTrackData track = TestCards.AsDebit(TestCards.VisaSwipe(), "32539F50C245A6A93D123412324000AA");

            Transaction response = track.Charge(142m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reversal = track.Reverse(142m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Lodging_099_DebitReversal_Master() {
            DebitTrackData track = TestCards.AsDebit(TestCards.MasterCardSwipe(), "F505AD81659AA42A3D123412324000AB");

            Transaction response = track.Charge(44m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);

            Transaction reversal = track.Reverse(33m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        /*
            CONTACTLESS - Sales
        */
        [TestMethod]
        public void Lodging_100_ContactlessSale_Visa() {
            track = TestCards.VisaSwipe(EntryMethod.Proximity);

            Transaction response = track.Charge(6m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_101_ContactlessSale_Master() {
            track = TestCards.MasterCardSwipe(EntryMethod.Proximity);

            Transaction response = track.Charge(6m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_102_ContactlessSale_Discover()
        {
            track = TestCards.DiscoverSwipe(EntryMethod.Proximity);

            Transaction response = track.Charge(6m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_103_ContactlessSale_Amex() {
            track = TestCards.AmexSwipe(EntryMethod.Proximity);

            Transaction response = track.Charge(9m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            TIME OUT REVERSAL (TOR)
        */
        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void Lodging_104_TimeOutReversal_Discover() {
            track = TestCards.DiscoverSwipe();

            string clientTransactionId = new Random().Next(10000000, 90000000).ToString();

            track.Charge(10.33m)
                    .WithCurrency("USD")
                    .WithClientTransactionId(clientTransactionId)
                    .Execute();

            var response = Transaction.FromId(null, PaymentMethodType.Credit);
            response.ClientTransactionId = clientTransactionId;

            var reversalResponse = response.Reverse(10.33m).Execute();
            Assert.IsNotNull(reversalResponse);
            Assert.AreEqual("00", reversalResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void Lodging_105_TimeOutReversal_Master() {
            track = TestCards.MasterCardSwipe();

            string clientTransactionId = new Random().Next(10000000, 90000000).ToString();

            track.Charge(10.33m)
                    .WithCurrency("USD")
                    .WithClientTransactionId(clientTransactionId)
                    .Execute();

            var response = Transaction.FromId(null, PaymentMethodType.Credit);
            response.ClientTransactionId = clientTransactionId;

            var reversalResponse = response.Reverse(10.33m).Execute();
            Assert.IsNotNull(reversalResponse);
            Assert.AreEqual("00", reversalResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void Lodging_106_TimeOutReversal_Jcb() {
            track = TestCards.JcbSwipe();

            string clientTransactionId = new Random().Next(10000000, 90000000).ToString();

            track.Charge(10.33m)
                    .WithCurrency("USD")
                    .WithClientTransactionId(clientTransactionId)
                    .Execute();

            var response = Transaction.FromId(null, PaymentMethodType.Credit);
            response.ClientTransactionId = clientTransactionId;

            var reversalResponse = response.Reverse(10.33m).Execute();
            Assert.IsNotNull(reversalResponse);
            Assert.AreEqual("00", reversalResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void Lodging_107_TimeOutReversal_Visa() {
            track = TestCards.VisaSwipe();

            string clientTransactionId = new Random().Next(10000000, 90000000).ToString();

            track.Charge(10.33m)
                    .WithCurrency("USD")
                    .WithClientTransactionId(clientTransactionId)
                    .Execute();

            var response = Transaction.FromId(null, PaymentMethodType.Credit);
            response.ClientTransactionId = clientTransactionId;

            var reversalResponse = response.Reverse(10.33m).Execute();
            Assert.IsNotNull(reversalResponse);
            Assert.AreEqual("00", reversalResponse.ResponseCode);
        }

        [TestMethod]
        public void Lodging_999_CloseBatch() {
            try {
                var response = BatchService.CloseBatch();
                Assert.IsNotNull(response);
                Debug.WriteLine(string.Format("Batch ID: {0}", response.Id));
                Debug.WriteLine(string.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (GatewayException exc) {
                if (exc.ResponseMessage != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }

        /*
           		Per GSTP ISO Lodging MSRDebit Purchase with Cash Back
       */

        [TestMethod]
        public void Lodging_246_DebitSale_Master() {
            DebitTrackData track = TestCards.AsDebit(TestCards.MasterCardSwipe(), "F505AD81659AA42A3D123412324000AB");

            Transaction response = track.Charge(3.73m)
                    .WithCurrency("USD")
                    .WithCashBack(0.40m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Lodging_247_DebitSaleVisaKeyed_CardPresent() {
            card = TestCards.VisaManual(true, true);

            Transaction response = card.Charge(3.70m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction duplicateResponse = card.Charge(3.70m)
                   .WithCurrency("USD")
                   .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", duplicateResponse.ResponseCode);
        }
    }
}