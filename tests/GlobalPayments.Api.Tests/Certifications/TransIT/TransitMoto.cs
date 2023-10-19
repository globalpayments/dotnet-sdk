using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Certifications.TransIT {
    public class TransitMoto : TransitBaseTestClass {
        /*
         * Step 1 Occurs in the background
         */
        public TransitMoto() : base() {
        }

        #region Step 2: Phone or Mail Transactions 

        [TestMethod]
        public void Test_001_Visa_Level_II_Sale() {
            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };

            Transaction response = VisaManual.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_MasterCard_Level_II_Sale() {
            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 0.02m,
                PoNumber = "9876543210"
            };

            Transaction response = MasterCardManual.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_Amex_Level_II_Sale() {
            var commercialData = new CommercialData(TaxType.NOTUSED) {
                SupplierReferenceNumber = "123456",
                CustomerReferenceId = "987654",
                DestinationPostalCode = "85284",
                Description = "AMEX LEVEL 2 TEST CASE"
            };

            Transaction response = AmexManual.Charge(1.5m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_004_MasterCard_BIN2_Sale() {
            Transaction response = MasterCardBin2Manual.Charge(11.1m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(5.55m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Test_005_Discover_Sale() {
            Transaction response = DiscoverManual.Charge(12m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006_Diners_Auth() {
            Transaction response = DinersManual.Authorize(6m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007_MasterCard_Sale() {
            Transaction response = MasterCardManual.Charge(15m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction reversal = response.Reverse(5m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_008_MasterCard_Sale() {
            Transaction response = MasterCardManual.Charge(34.13m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_009_JCB_Sale() {
            Transaction response = JcbManual.Charge(13m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_010_Amex_Sale() {
            var card = new CreditCardData {
                Number = "371449635392376",
                ExpMonth = 12,
                ExpYear = 2020
            };

            Transaction response = card.Charge(13.5m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_011_Visa_Sale() {
            Transaction response = VisaManual.Charge(32.49m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_012_Discover_CUP_Sale() {
            Transaction response = DiscoverCupManual.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_013_Visa_Sale() {
            Transaction response = VisaManual.Charge(11.12m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_014_Amex_Sale() {
            Transaction response = AmexManual.Charge(4m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        #endregion

        #region Step 3: CardAuthentication Tests

        [TestMethod]
        public void Test_015_Visa_Verfiy() {
            Transaction response = VisaManual.Verify()
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_016_MasterCard_Verfiy() {
            Transaction response = MasterCardManual.Verify()
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_017_Amex_Verfiy() {
            Transaction response = AmexManual.Verify()
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Step 4: LEVEL 3 Enhanced Data Tests

        [TestMethod]
        public void Test_018_Visa_Level3_Sale() {
            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };
            commercialData.AddLineItems(
                new CommercialLineItem {
                    CommodityCode = "030",
                    Description = "Misc Good",
                    UPC = "012345678901",
                    Quantity = 5.1m,
                    UnitOfMeasure = "lbs",
                    UnitCost = 0.6m,
                    //DiscountAmount = 0.61m,
                    TotalAmount = 0.62m,
                    TaxAmount = 0.63m,
                    ExtendedAmount = 0.64m,
                    CreditDebitIndicator = CreditDebitIndicator.Credit,
                    NetGrossIndicator = NetGrossIndicator.Gross
                },
                new CommercialLineItem {
                    CommodityCode = "031",
                    Description = "Misc Good2",
                    UPC = "012345678901",
                    Quantity = 5.1m,
                    UnitOfMeasure = "lbs",
                    UnitCost = 0.6m,
                    //DiscountAmount = 0.61m,
                    TotalAmount = 0.62m,
                    TaxAmount = 0.63m,
                    ExtendedAmount = 0.64m,
                    CreditDebitIndicator = CreditDebitIndicator.Credit,
                    NetGrossIndicator = NetGrossIndicator.Gross
                }
            );

            Transaction response = VisaManual.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_019_MasterCard_Level3_Sale() {
            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };
            commercialData.AddLineItems(
                 new CommercialLineItem {
                     CommodityCode = "030",
                     Description = "Misc Good",
                     UPC = "012345678901",
                     Quantity = 5.1m,
                     UnitOfMeasure = "lbs",
                     UnitCost = 0.6m,
                     //DiscountAmount = 0.61m,
                     TotalAmount = 0.62m,
                     TaxAmount = 0.63m,
                     ExtendedAmount = 0.64m,
                     CreditDebitIndicator = CreditDebitIndicator.Credit,
                     NetGrossIndicator = NetGrossIndicator.Gross
                 }
             );

            Transaction response = MasterCardManual.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Step 5: Void Transactions

        // TEST 20 Partial Reversal: See Test_007_MasterCard_Sale

        // TEST 21 Full Reversal: See Test_014_Amex_Sale

        #endregion

        #region Step 6: 3D Secure and UCSF Transactions

        [TestMethod]
        public void Test_022_Visa_Internet_ECI_5() {
            VisaManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "1234567890123456789012345678901234567890"
            };

            Transaction response = VisaManual.Charge(1.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_023_Mastercard_Internet() {
            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "12345678901234567890123456789012",
                //UcafCollectionIndicator = UcafCollectionIndicator.Supported_Authenticated
            };

            Transaction response = MasterCardManual.Charge(34.02m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_024_Discover_Internet() {
            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "12345678901234567890123456789012345678901234567890123456789012345678901234567890"
            };

            Transaction response = DiscoverManual.Charge(45.02m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_025_AMEX_Internet() {
            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "12345678901234567890123456789012345678901234567890123456789012345678901234567890"
            };

            Transaction response = AmexManual.Charge(32.02m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_026_Visa_Internet_ECI_6() {
            VisaManual.ThreeDSecure = new ThreeDSecure {
                Eci = "6",
                AuthenticationValue = "1234567890123456789012345678901234567890"
            };

            Transaction response = VisaManual.Charge(0.81m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_027_Mastercard_Internet() {
            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "12345678901234567890123456789012",
                //UcafCollectionIndicator = UcafCollectionIndicator.Supported_Authenticated
            };

            Transaction response = MasterCardManual.Charge(29m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Step 7: CIT/COF Transactions

        [TestMethod]
        public void Test_028_Visa() {
            // string token = VisaManual.Tokenize();

            Transaction response = VisaToken.Charge(14m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_029_Mastercard() {
            // string token = MasterCardManual.Tokenize();

            Transaction response = MasterCardToken.Charge(15m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Step 8: Multi-Clearing/Shipping

        [TestMethod]
        public void Test_030_Visa_MultiPart() {
            // AUTH
            Transaction response = VisaManual.Authorize(30m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // FIRST CAPTURE
            Transaction firstCapture = response.Capture(15m)
                .WithMultiCapture(1, 2)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);

            // SECOND CAPTURE
            Transaction secondCapture = response.Capture(15m)
                .WithMultiCapture(2, 2)
                .Execute();
            Assert.IsNotNull(secondCapture);
            Assert.AreEqual("00", secondCapture.ResponseCode);
        }

        [TestMethod]
        public void Test_031_MasterCard_MultiPart() {
            // AUTH
            Transaction response = MasterCardManual.Authorize(50m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // FIRST CAPTURE
            Transaction firstCapture = response.Capture(30m)
                .WithMultiCapture(1, 3)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);

            // SECOND CAPTURE
            Transaction secondCapture = response.Capture(10m)
                .WithMultiCapture(2, 3)
                .Execute();
            Assert.IsNotNull(secondCapture);
            Assert.AreEqual("00", secondCapture.ResponseCode);

            // SECOND CAPTURE
            Transaction thirdCapture = response.Capture(10m)
                .WithMultiCapture(2, 3)
                .Execute();
            Assert.IsNotNull(thirdCapture);
            Assert.AreEqual("00", thirdCapture.ResponseCode);
        }

        [TestMethod]
        public void Test_032_MasterCard_Single_Shipment() {
            // AUTH
            Transaction response = MasterCardManual.Authorize(60m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // FIRST CAPTURE
            Transaction firstCapture = response.Capture(60m)
                .WithMultiCapture(1, 1)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);
        }

        #endregion

        #region Settlement

        [TestMethod]
        public void Test_033_Settlement() {
            BatchSummary response = BatchService.CloseBatch();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion
    }
}
