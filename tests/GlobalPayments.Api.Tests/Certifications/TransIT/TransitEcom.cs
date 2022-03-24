using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;

namespace GlobalPayments.Api.Tests.Certifications.TransIT {
    [TestClass]
    public class TransitEcom : TransitBaseTestClass {
        protected override AcceptorConfig AcceptorConfig {
            get {
                return new AcceptorConfig {
                    CardDataInputCapability = CardDataInputCapability.KeyEntry,
                    OperatingEnvironment = OperatingEnvironment.OffPremises_CardAcceptor_Unattended,
                    TerminalOutputCapability = TerminalOutputCapability.Display,
                    PinCaptureCapability = PinCaptureCapability.None,
                    CardCaptureCapability = false,
                    CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated
                };
            }
        }

        /*
         * Step 1 Occurs in the background
         */
        public TransitEcom() : base() {           
        }        

        #region Step 2: INTERNET e-Commerce Transactions 

        [TestMethod]
        public void Test_001_Visa_Level_II_Sale() {
            Logger.AppendText("Test_001_Visa_Level_II_Sale");

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                PoNumber = "9876543210",
                TaxAmount = 0m
            };

            Response = VisaManual.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .WithDescription("Test_001_Visa_Level_II_Sale")
                .Execute();
        }

        [TestMethod]
        public void Test_002_MasterCard_Level_II_Sale() {
            Logger.AppendText("\r\nTest_002_MasterCard_Level_II_Sale");

            var commercialData = new CommercialData(TaxType.SALESTAX) {
                TaxAmount = 0.02m,
                PoNumber = "9876543210"
            };

            Response = MasterCardManual.Charge(0.52m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_003_Amex_Level_II_Sale() {
            Logger.AppendText("\r\nTest_003_Amex_Level_II_Sale");

            var commercialData = new CommercialData(TaxType.NOTUSED) {
                SupplierReferenceNumber = "123456",
                CustomerReferenceId = "987654",
                DestinationPostalCode = "85284",
                Description = "AMEX LEVEL 2 TEST CASE",
                TaxAmount = 0m
            };

            Response = AmexManual.Charge(1.5m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_004_MasterCard_BIN2_Sale() {
            Logger.AppendText("\r\nTest_004_MasterCard_BIN2_Sale");

            Response = MasterCardBin2Manual.Charge(11.1m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
            Assert.AreEqual(5.55m, Response?.AuthorizedAmount);
        }

        [TestMethod]
        public void Test_005_Discover_Sale() {
            Logger.AppendText("\r\nTest_005_Discover_Sale");

            Response = DiscoverManual.Charge(12m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod, Ignore]
        public void Test_006_Diners_Auth() {
            Logger.AppendText("\r\nTest_006_Diners_Auth");

            Response = DinersManual.Authorize(6m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_007_MasterCard_Sale() {
            Logger.AppendText("\r\nTest_007_MasterCard_Sale");

            Response = MasterCardManual.Charge(15m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(Response);
            Assert.AreEqual("00", Response.ResponseCode);

            Logger.AppendText("\r\nTest_020_Partial_Void");
           
            Response = Response.Reverse(5m)
                .WithCurrency("USD")
                .WithVoidReason(VoidReason.PartialReversal)
                .Execute();
        }

        [TestMethod]
        public void Test_008_MasterCard_Sale() {
            Logger.AppendText("\r\nTest_008_MasterCard_Sale");

            Response = MasterCardManual.Charge(34.13m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_009_JCB_Sale() {
            Logger.AppendText("\r\nTest_009_JCB_Sale");

            Response = JcbManual.Charge(13m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_010_Amex_Sale() {
            Logger.AppendText("\r\nTest_010_Amex_Sale");

            var card = new CreditCardData {
                Number = "371449635392376",
                ExpMonth = 12,
                ExpYear = 2020
            };

            Response = card.Charge(13.5m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_011_Visa_Sale() {
            Logger.AppendText("\r\nTest_011_Visa_Sale");

            Response = VisaManual.Charge(32.49m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_012_Discover_CUP_Sale() {
            Logger.AppendText("\r\nTest_012_Discover_CUP_Sale");

            Response = DiscoverCupManual.Charge(10m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_013_Visa_Sale() {
            Logger.AppendText("\r\nTest_013_Visa_Sale");

            Response = VisaManual.Charge(11.12m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_014_Amex_Sale() {
            Logger.AppendText("\r\nTest_014_Amex_Sale");

            Response = AmexManual.Charge(4m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(Response);
            Assert.AreEqual("00", Response.ResponseCode);

            Logger.AppendText("\r\nTest_021_Full_Void");

            Response = Response.Void()
                .WithVoidReason(VoidReason.PostAuth_UserDeclined)
                .Execute();
        }

        #endregion

        #region Step 3: CardAuthentication Tests

        [TestMethod]
        public void Test_015_Visa_Verfiy() {
            Logger.AppendText("\r\nTest_015_Visa_Verfiy");

            Response = VisaManual.Verify()
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod]
        public void Test_016_MasterCard_Verfiy() {
            Logger.AppendText("\r\nTest_016_MasterCard_Verfiy");

            Response = MasterCardManual.Verify()
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod]
        public void Test_017_Amex_Verfiy() {
            Logger.AppendText("\r\nTest_017_Amex_Verfiy");

            Response = AmexManual.Verify()
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        #endregion

        #region Step 4: LEVEL 3 Enhanced Data Tests

        [TestMethod]
        public void Test_018_Visa_Level3_Sale() {
            Logger.AppendText("\r\nTest_018_Visa_Level3_Sale");

            var commercialData = new CommercialData(TaxType.NOTUSED, CommercialIndicator.Level_III) {
                PoNumber = "1784951399984509620",
                TaxAmount = 0.01m,
                DestinationPostalCode = "85212",
                DestinationCountryCode = "USA",
                OriginPostalCode = "22193",
                SummaryCommodityCode = "SCC",
                CustomerVAT_Number = "123456789",
                VAT_InvoiceNumber = "UVATREF162",
                OrderDate = DateTime.Now,
                FreightAmount = 0.01m,
                DutyAmount = 0.01m,
                AdditionalTaxDetails = new AdditionalTaxDetails {
                    TaxType = "VAT",
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m,
                    TaxCategory = TaxCategory.VAT
                }
            };
            commercialData.AddLineItems(
                new CommercialLineItem {
                    ProductCode = "PRDCD1",
                    Name = "PRDCD1NAME",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 1 NOTES",
                    CommodityCode = "12DIGIT ACCO",
                    AlternateTaxId = "1234567890",
                    CreditDebitIndicator = CreditDebitIndicator.Credit,
                    DiscountDetails = new DiscountDetails {
                        DiscountName = "Indep Sale 1",
                        DiscountAmount = 0.50m,
                        DiscountPercentage = 0.10m,
                        DiscountType = "SALE"
                    }
                },
                new CommercialLineItem {
                    ProductCode = "PRDCD2",
                    Name = "PRDCD2NAME",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 2 NOTES",
                    CommodityCode = "12DIGIT ACCO",
                    AlternateTaxId = "1234567890",
                    CreditDebitIndicator = CreditDebitIndicator.Debit,
                    DiscountDetails = new DiscountDetails {
                        DiscountName = "Indep Sale 1",
                        DiscountAmount = 0.50m,
                        DiscountPercentage = 0.10m,
                        DiscountType = "SALE"
                    }
                }
            );

            Response = VisaManual.Charge(0.53m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        [TestMethod]
        public void Test_019_MasterCard_Level3_Sale() {
            Logger.AppendText("\r\nTest_019_MasterCard_Level3_Sale");

            var commercialData = new CommercialData(TaxType.NOTUSED, CommercialIndicator.Level_III) {
                PoNumber = "9876543210",
                TaxAmount = 0.01m,
                DestinationPostalCode = "85212",
                DestinationCountryCode = "USA",
                OriginPostalCode = "22193",
                SummaryCommodityCode = "SCC",
                CustomerVAT_Number = "123456789",
                VAT_InvoiceNumber = "UVATREF162",
                OrderDate = DateTime.Now,
                FreightAmount = 0.01m,
                DutyAmount = 0.01m,
                AdditionalTaxDetails = new AdditionalTaxDetails {
                    TaxType = "VAT",
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m,
                    TaxCategory = TaxCategory.VAT
                }
            };
            commercialData.AddLineItems(
                 new CommercialLineItem {
                     ProductCode = "PRDCD1",
                     Name = "PRDCD1NAME",
                     UnitCost = 0.01m,
                     Quantity = 1m,
                     UnitOfMeasure = "METER",
                     Description = "PRODUCT 1 NOTES",
                     CommodityCode = "12DIGIT ACCO",
                     AlternateTaxId = "1234567890",
                     CreditDebitIndicator = CreditDebitIndicator.Credit,
                     DiscountDetails = new DiscountDetails {
                        DiscountName = "Indep Sale 1",
                        DiscountAmount = 0.50m,
                        DiscountPercentage = 0.10m,
                        DiscountType = "SALE"
                    }
                 }
             );

            Response = MasterCardManual.Charge(0.53m)
                .WithCurrency("USD")
                .WithCommercialData(commercialData)
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
        }

        #endregion

        #region Step 5: Void Transactions

        // TEST 20 Partial Reversal: See Test_007_MasterCard_Sale

        // TEST 21 Full Reversal: See Test_014_Amex_Sale

        #endregion

        #region Step 6a: 3D Secure 1 and UCSF Transactions

        [TestMethod, Ignore]
        public void Test_022a_Visa_Internet_ECI_5() {
            Logger.AppendText("\r\nTest_022_Visa_Internet_ECI_5");

            VisaManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "1234567890123456789012345678901234567890",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated,
                Version = Secure3dVersion.One
            };

            Response = VisaManual.Charge(1.01m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod, Ignore]
        public void Test_023a_Mastercard_Internet() {
            Logger.AppendText("\r\nTest_023_Mastercard_Internet");

            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "12345678901234567890123456789012",
                AuthenticationType = "24", // I believe this is 3DS
                UCAFIndicator = UCAFIndicator.FullyAuthenticated,
                Version = Secure3dVersion.One
            };

            Response = MasterCardManual.Charge(34.02m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod, Ignore]
        public void Test_024a_Discover_Internet() {
            Logger.AppendText("\r\nTest_024_Discover_Internet");

            DiscoverManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "1234567890123456789012345678901234567890",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated,
                Version = Secure3dVersion.One
            };

            Response = DiscoverManual.Charge(45.02m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod, Ignore]
        public void Test_025a_AMEX_Internet() {
            Logger.AppendText("\r\nTest_025_AMEX_Internet");

            AmexManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "1234567890123456789012345678901234567890",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated,
                Version = Secure3dVersion.One
            };

            Response = AmexManual.Charge(32.02m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod, Ignore]
        public void Test_026a_Visa_Internet_ECI_6() {
            Logger.AppendText("\r\nTest_026_Visa_Internet_ECI_6");

            VisaManual.ThreeDSecure = new ThreeDSecure {
                Eci = "6",
                SecureCode = "1234567890123456789012345678901234567890",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated,
                Version = Secure3dVersion.One
            };

            Response = VisaManual.Charge(0.81m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod, Ignore]
        public void Test_027a_Mastercard_Internet() {
            Logger.AppendText("\r\nTest_027_Mastercard_Internet");

            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "12345678901234567890123456789012",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated,
                Version = Secure3dVersion.One
            };

            Response = MasterCardManual.Charge(29m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        #endregion

        #region Step 6b: 3D Secure 2 and UCSF Transactions

        [TestMethod]
        public void Test_022b_Visa_Internet_ECI_5() {
            Logger.AppendText("\r\nTest_022_Visa_Internet_ECI_5");

            VisaManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "1234567890123456789012345678901234567890",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated
            };

            Response = VisaManual.Charge(1.01m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod]
        public void Test_023b_Mastercard_Internet() {
            Logger.AppendText("\r\nTest_023_Mastercard_Internet");

            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                AuthenticationValue = "ODQzNjgwNjU0ZjM3N2JmYTg0NTM=",
                AuthenticationType = "24",
                DirectoryServerTransactionId = "c272b04f-6e7b-43a2-bb78-90f4fb94aa25",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated
            };

            Response = MasterCardManual.Charge(34.02m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod]
        public void Test_024b_Discover_Internet() {
            Logger.AppendText("\r\nTest_024_Discover_Internet");

            DiscoverManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "1234567890123456789012345678901234567890",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated
            };

            Response = DiscoverManual.Charge(45.02m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod]
        public void Test_025b_AMEX_Internet() {
            Logger.AppendText("\r\nTest_025_AMEX_Internet");

            AmexManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "1234567890123456789012345678901234567890",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated
            };

            Response = AmexManual.Charge(32.02m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod]
        public void Test_026b_Visa_Internet_ECI_6() {
            Logger.AppendText("\r\nTest_026_Visa_Internet_ECI_6");

            VisaManual.ThreeDSecure = new ThreeDSecure {
                Eci = "6",
                SecureCode = "1234567890123456789012345678901234567890",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated
            };

            Response = VisaManual.Charge(0.81m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod]
        public void Test_027b_Mastercard_Internet() {
            Logger.AppendText("\r\nTest_027_Mastercard_Internet");

            MasterCardManual.ThreeDSecure = new ThreeDSecure {
                Eci = "5",
                SecureCode = "12345678901234567890123456789012",
                AuthenticationType = "24",
                DirectoryServerTransactionId = "c272b04f-6e7b-43a2-bb78-90f4fb94aa25",
                UCAFIndicator = UCAFIndicator.FullyAuthenticated
            };

            Response = MasterCardManual.Charge(29m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        #endregion

        #region Step 7: CIT/COF Transactions

        [TestMethod, Ignore]
        public void Test_028_Visa() {
            Logger.AppendText("\r\nTest_028_Visa");

            Response = VisaToken.Charge(14m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        [TestMethod, Ignore]
        public void Test_029_Mastercard() {
            Logger.AppendText("\r\nTest_029_Mastercard");

            Response = MasterCardToken.Charge(15m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
        }

        #endregion

        #region Step 8: Multi-Clearing/Shipping

        [TestMethod]
        public void Test_030_Visa_MultiPart() {
            Logger.AppendText("\r\nTest_030_Visa_MultiPart");

            // AUTH
            Response = VisaManual.Authorize(30m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
            Assert.IsNotNull(Response);
            Assert.AreEqual("00", Response.ResponseCode);

            // FIRST CAPTURE
            Logger.AppendText("\r\nTest_030_First_Capture");

            Transaction firstCapture = Response.Capture(15m)
                .WithMultiCapture(1, 2)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);

            // SECOND CAPTURE
            Logger.AppendText("\r\nTest_030_Second_Capture");

            Transaction secondCapture = Response.Capture(15m)
                .WithMultiCapture(2, 2)
                .Execute();
            Assert.IsNotNull(secondCapture);
            Assert.AreEqual("00", secondCapture.ResponseCode);
        }

        [TestMethod]
        public void Test_031_MasterCard_MultiPart() {
            // AUTH
            Logger.AppendText("\r\nTest_031_MasterCard_MultiPart");

            Response = MasterCardManual.Authorize(50m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
            Assert.IsNotNull(Response);
            Assert.AreEqual("00", Response.ResponseCode);

            // FIRST CAPTURE
            Logger.AppendText("\r\nTest_031_First_Capture");

            Transaction firstCapture = Response.Capture(30m)
                .WithMultiCapture(1, 3)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);

            // SECOND CAPTURE
            Logger.AppendText("\r\nTest_031_Second_Capture");

            Transaction secondCapture = Response.Capture(10m)
                .WithMultiCapture(2, 3)
                .Execute();
            Assert.IsNotNull(secondCapture);
            Assert.AreEqual("00", secondCapture.ResponseCode);

            // THIRD CAPTURE
            Logger.AppendText("\r\nTest_031_Third_Capture");

            Transaction thirdCapture = Response.Capture(10m)
                .WithMultiCapture(3, 3)
                .Execute();
            Assert.IsNotNull(thirdCapture);
            Assert.AreEqual("00", thirdCapture.ResponseCode);
        }

        [TestMethod]
        public void Test_032_MasterCard_Single_Shipment() {
            // AUTH
            Logger.AppendText("\r\nTest_032_MasterCard_Single_Shipment");

            Response = MasterCardManual.Authorize(60m)
                .WithCurrency("USD")
                .WithAddress(Address)
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
            Assert.IsNotNull(Response);
            Assert.AreEqual("00", Response.ResponseCode);

            // FIRST CAPTURE
            Logger.AppendText("\r\nTest_032_First_Capture");

            Transaction firstCapture = Response.Capture(60m)
                .WithMultiCapture(1, 1)
                .Execute();
            Assert.IsNotNull(firstCapture);
            Assert.AreEqual("00", firstCapture.ResponseCode);
        }

        #endregion

        #region Settlement

        [TestMethod]
        public void Test_999_Settlement() {
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
