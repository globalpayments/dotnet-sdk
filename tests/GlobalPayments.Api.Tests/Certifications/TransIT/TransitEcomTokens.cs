using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.Certifications.TransIT {
    [TestClass]
    public class TransitEcomTokens : TransitBaseTestClass {
        public TransitEcomTokens() : base() { }

        #region Step 2: INTERNET e-Commerce Transactions 

        [TestMethod]
        public void Test_001_Visa_Level_II_Sale() {
            Logger.AppendText("\r\nTest_001_Visa_Level_II_Sale");

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210"
            };

            Transaction response = VisaToken.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_MasterCard_Level_II_Sale() {
            Logger.AppendText("\r\nTest_002_MasterCard_Level_II_Sale");

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 0.02m,
                PoNumber = "9876543210"
            };

            Transaction response = MasterCardToken.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_Amex_Level_II_Sale() {
            Logger.AppendText("\r\nTest_003_Amex_Level_II_Sale");

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                SupplierReferenceNumber = "123456",
                CustomerReferenceId = "987654",
                DestinationPostalCode = "85284",
                Description = "AMEX LEVEL 2 TEST CASE"
            };

            Transaction response = AmexToken.Charge(1.5m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_004_MasterCard_BIN2_Sale() {
            Logger.AppendText("\r\nTest_004_MasterCard_BIN2_Sale");

            Transaction response = MasterCardBin2Token.Charge(11.1m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(5.55m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Test_005_Discover_Sale() {
            Logger.AppendText("\r\nTest_005_Discover_Sale");

            Transaction response = DiscoverToken.Charge(12m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void Test_006_Diners_Auth() {
            Logger.AppendText("\r\nTest_006_Diners_Auth");

            Transaction response = DinersToken.Authorize(6m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007_MasterCard_Sale() {
            Logger.AppendText("\r\nTest_007_MasterCard_Sale");

            Transaction response = MasterCardToken.Charge(15m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Logger.AppendText("\r\nTest_020_Partial_Void");

            Transaction reversal = response.Reverse(5m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_008_MasterCard_Sale() {
            Logger.AppendText("\r\nTest_008_MasterCard_Sale");

            Transaction response = MasterCardToken.Charge(34.13m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_009_JCB_Sale() {
            Logger.AppendText("\r\nTest_009_JCB_Sale");

            Transaction response = JcbToken.Charge(13m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_010_Amex_Sale() {
            Logger.AppendText("\r\nTest_010_Amex_Sale");

            Transaction response = AmexToken.Charge(13.5m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_011_Visa_Sale() {
            Logger.AppendText("\r\nTest_011_Visa_Sale");

            Transaction response = VisaToken.Charge(32.49m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_012_Discover_CUP_Sale() {
            Logger.AppendText("\r\nTest_012_Discover_CUP_Sale");

            Transaction response = DiscoverCupToken.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_013_Visa_Sale() {
            Logger.AppendText("\r\nTest_013_Visa_Sale");

            Transaction response = VisaToken.Charge(11.12m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_014_Amex_Sale() {
            Logger.AppendText("\r\nTest_014_Amex_Sale");

            Transaction response = AmexToken.Charge(4m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);


            Logger.AppendText("\r\nTest_021_Full_Void");

            Transaction voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        #endregion

        #region Step 3: CardAuthentication Tests

        [TestMethod]
        public void Test_015_Visa_Verfiy() {
            Logger.AppendText("\r\nTest_015_Visa_Verfiy");

            Transaction response = VisaToken.Verify()
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_016_MasterCard_Verfiy() {
            Logger.AppendText("\r\nTest_016_MasterCard_Verfiy");

            Transaction response = MasterCardToken.Verify()
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_017_Amex_Verfiy() {
            Logger.AppendText("\r\nTest_017_Amex_Verfiy");

            Transaction response = AmexToken.Verify()
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Step 4: LEVEL 3 Enhanced Data Tests

        [TestMethod]
        public void Test_018_Visa_Level3_Sale() {
            Logger.AppendText("\r\nTest_018_Visa_Level3_Sale");

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

            Transaction response = VisaToken.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_019_MasterCard_Level3_Sale() {
            Logger.AppendText("\r\nTest_019_MasterCard_Level3_Sale");

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

            Transaction response = MasterCardToken.Charge(0.52m)
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
            Logger.AppendText("\r\nTest_022_Visa_Internet_ECI_5");

            VisaManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "1234567890123456789012345678901234567890"
            };

            Transaction response = VisaToken.Charge(1.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_023_Mastercard_Internet() {
            Logger.AppendText("\r\nTest_023_Mastercard_Internet");

            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "12345678901234567890123456789012",
                //UcafCollectionIndicator = UcafCollectionIndicator.Supported_Authenticated
            };

            Transaction response = MasterCardToken.Charge(34.02m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_024_Discover_Internet() {
            Logger.AppendText("\r\nTest_024_Discover_Internet");

            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "12345678901234567890123456789012345678901234567890123456789012345678901234567890"
            };

            Transaction response = DiscoverToken.Charge(45.02m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_025_AMEX_Internet() {
            Logger.AppendText("\r\nTest_025_AMEX_Internet");

            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "12345678901234567890123456789012345678901234567890123456789012345678901234567890"
            };

            Transaction response = AmexToken.Charge(32.02m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_026_Visa_Internet_ECI_6() {
            Logger.AppendText("\r\nTest_026_Visa_Internet_ECI_6");

            VisaManual.ThreeDSecure = new ThreeDSecure {
                Eci = "6",
                AuthenticationValue = "1234567890123456789012345678901234567890"
            };

            Transaction response = VisaToken.Charge(0.81m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_027_Mastercard_Internet() {
            Logger.AppendText("\r\nTest_027_Mastercard_Internet");

            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "12345678901234567890123456789012",
                //UcafCollectionIndicator = UcafCollectionIndicator.Supported_Authenticated
            };

            Transaction response = MasterCardToken.Charge(29m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        #endregion

        #region Step 7: CIT/COF Transactions

        [TestMethod]
        public void Test_028_Visa() {
            Logger.AppendText("\r\nTest_028_Visa");

            Transaction response = VisaToken.Charge(14m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Test_029_Mastercard() {
            Logger.AppendText("\r\nTest_029_Mastercard");

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
            Logger.AppendText("\r\nTest_030_Visa_MultiPart");

            // AUTH
            Transaction response = VisaToken.Authorize(30m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // FIRST CAPTURE
            Logger.AppendText("\r\nTest_030_First_Capture");

            Transaction firstCapture = response.Capture(15m)
                .WithMultiCapture(1, 2)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);

            // SECOND CAPTURE
            Logger.AppendText("\r\nTest_030_Second_Capture");

            Transaction secondCapture = response.Capture(15m)
                .WithMultiCapture(2, 2)
                .Execute();
            Assert.IsNotNull(secondCapture);
            Assert.AreEqual("00", secondCapture.ResponseCode);
        }

        [TestMethod]
        public void Test_031_MasterCard_MultiPart() {
            // AUTH
            Logger.AppendText("\r\nTest_031_MasterCard_MultiPart");

            Transaction response = MasterCardToken.Authorize(50m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // FIRST CAPTURE
            Logger.AppendText("\r\nTest_031_First_Capture");

            Transaction firstCapture = response.Capture(30m)
                .WithMultiCapture(1, 3)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);

            // SECOND CAPTURE
            Logger.AppendText("\r\nTest_031_Second_Capture");

            Transaction secondCapture = response.Capture(10m)
                .WithMultiCapture(2, 3)
                .Execute();
            Assert.IsNotNull(secondCapture);
            Assert.AreEqual("00", secondCapture.ResponseCode);

            // THIRD CAPTURE
            Logger.AppendText("\r\nTest_031_Third_Capture");

            Transaction thirdCapture = response.Capture(10m)
                .WithMultiCapture(2, 3)
                .Execute();
            Assert.IsNotNull(thirdCapture);
            Assert.AreEqual("00", thirdCapture.ResponseCode);
        }

        [TestMethod]
        public void Test_032_MasterCard_Single_Shipment() {
            // AUTH
            Logger.AppendText("\r\nTest_032_MasterCard_Single_Shipment");

            Transaction response = MasterCardToken.Authorize(60m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // FIRST CAPTURE
            Logger.AppendText("\r\nTest_032_First_Capture");

            Transaction firstCapture = response.Capture(60m)
                .WithMultiCapture(1, 1)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);
        }

        #endregion

        #region Step 9: Tokenize Cards

        [TestMethod]
        public void Test_033_Tokenize_Visa() {
            Logger.AppendText("\r\nTest_033_Tokenize_Visa");

            string token = VisaManual.Tokenize(false);
            Assert.IsNotNull(token);
            Logger.AppendText("Visa Token: {0}", token);
        }

        [TestMethod]
        public void Test_034_Tokenize_MasterCard() {
            Logger.AppendText("\r\nTest_034_Tokenize_MasterCard");

            string token = MasterCardManual.Tokenize(false);
            Assert.IsNotNull(token);
            Logger.AppendText("MasterCard Token: {0}", token);
        }

        [TestMethod]
        public void Test_035_Tokenize_MasterCard_BIN2() {
            Logger.AppendText("\r\nTest_035_Tokenize_MasterCard_BIN2");

            string token = MasterCardBin2Manual.Tokenize(false);
            Assert.IsNotNull(token);
            Logger.AppendText("MasterCard BIN2 Token: {0}", token);
        }

        [TestMethod]
        public void Test_036_Tokenize_Discover() {
            Logger.AppendText("\r\nTest_036_Tokenize_Discover");

            string token = DiscoverManual.Tokenize(false);
            Assert.IsNotNull(token);
            Logger.AppendText("Discover Token: {0}", token);
        }

        [TestMethod]
        public void Test_037_Tokenize_Amex() {
            Logger.AppendText("\r\nTest_037_Tokenize_Amex");

            string token = AmexManual.Tokenize(false);
            Assert.IsNotNull(token);
            Logger.AppendText("Amex Token: {0}", token);
        }

        [TestMethod]
        public void Test_038_Tokenize_JCB() {
            Logger.AppendText("\r\nTest_038_Tokenize_JCB");
            string token = JcbManual.Tokenize(false);
            Assert.IsNotNull(token);
            Logger.AppendText("JCB Token: {0}", token);
        }

        [TestMethod]
        public void Test_039_Tokenize_Discover_CUP() {
            Logger.AppendText("\r\nTest_039_Tokenize_Discover_CUP");

            string token = DiscoverCupManual.Tokenize(false);
            Assert.IsNotNull(token);
            Logger.AppendText("Discover CUP Token: {0}", token);
        }

        [TestMethod]
        public void Test_040_Tokenize_Diners() {
            Logger.AppendText("\r\nTest_040_Tokenize_Diners");

            string token = DinersManual.Tokenize(false);
            Assert.IsNotNull(token);
            Logger.AppendText("Diners Token: {0}", token);
        }

        #endregion

        #region Settlement

        [TestMethod]
        public void Test_000_Settlement() {
            Logger.AppendText("Authorization End Date/Time: {0}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            Logger.AppendText("First HRN: {0}", _firstTransactionId);
            Logger.AppendText("Last HRN: {0}", _lastTransactionId);

            Logger.AppendText("\r\nTest_000_Settlement");
            Logger.AppendText("Settlement Time: {0}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));

            BatchSummary response = BatchService.CloseBatch();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            //_logger.AppendText("Batch Number: {0}", response.BatchId);
        }

        #endregion
    }
}
