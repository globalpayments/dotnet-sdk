using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Genius {
    [TestClass]
    public class GeniusTests {
        private Address address;

        private CreditCardData applePay;
        private CreditCardData card;
        private CreditCardData encryptedCard;
        private CreditCardData tokenizedCard;
        private CreditTrackData track;

        public GeniusTests() {
            ServicesContainer.ConfigureService(new GeniusConfig {
                MerchantName = "Test Shane Logsdon",
                MerchantSiteId = "BKHV2T68",
                MerchantKey = "AT6AN-ALYJE-YF3AW-3M5NN-UQDG1",
                RegisterNumber = "35",
                TerminalId = "3"
            });

            address = new Address {
                StreetAddress1 = "1 Federal Street",
                PostalCode = "02110"
            };

            // TODO: Get Valid ApplePay Token
            applePay = new CreditCardData {
                Token = "ew0KCSJ2ZXJzaW9uIjogIkVDX3YxIiwNCgkiZ==",
                MobileType = MobilePaymentMethodType.APPLEPAY
            };

            card = TestCards.VisaManual();

            tokenizedCard = new CreditCardData {
                Token = "100000101GC58TDAUFDZ"
            };
            track = TestCards.VisaSwipe();

            encryptedCard = TestCards.VisaManual();
        }

        [TestMethod]
        public void AdjustTip() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var adjust = response.Edit()
                .WithGratuity(1m)
                .Execute();
            Assert.IsNotNull(adjust);
            Assert.AreEqual("00", adjust.ResponseCode);
        }

        [TestMethod, Ignore]
        public void AttachSignature() {
            // TODO: This needs some looking into... data seems terminal specific
        }

        [TestMethod]
        public void Authorize_Keyed() {
            var response = card.Authorize(10m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithInvoiceNumber("1556")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void Authorize_Encrypted_Keyed() {
            // TODO: Need some test data for this one
            var response = card.Authorize(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Authorize_Swiped() {
            var response = track.Authorize(10m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1264")
                .WithClientTransactionId("137149")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Authorize_Vault() {
            var response = tokenizedCard.Authorize(10m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1558")
                .WithClientTransactionId("167903")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void Authorize_Wallet() {
            var autoSubstantiation = new AutoSubstantiation {
                CopaySubTotal = 1m,
                ClinicSubTotal = 1m,
                DentalSubTotal = 1m,
                PrescriptionSubTotal = 1m,
                VisionSubTotal = 1m
            };

            var response = applePay.Authorize()
                .WithCurrency("USD")
                .WithAddress(address)
                .WithCashBack(1m)
                .WithConvenienceAmount(1.25m)
                .WithAutoSubstantiation(autoSubstantiation)
                .WithInvoiceNumber("INV123")
                //TaxAmount -> CommercialRequestData -> Level II
                //PoNumber -> CommercialRequestData -> Level II
                //CustomerCode
                .WithClientTransactionId("TX123")
                .WithAllowPartialAuth(true)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod, Ignore]
        public void Authorize_Level3() {
        }

        [TestMethod]
        public void BoardCard_Keyed() {
            string token = card.Tokenize();
            Assert.IsNotNull(token);
        }

        [TestMethod, Ignore]
        public void BoardCard_Encrypted_Keyed() {
            string token = encryptedCard.Tokenize();
            Assert.IsNotNull(token);
        }

        [TestMethod, Ignore]
        public void BoardCard_Transaction() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // TODO: This needs to be implemented on the transaction object
            //var token = response.Tokenize();
            //Assert.IsNotNull(token);
        }

        [TestMethod]
        public void BoardCard_Swiped() {
            string token = track.Tokenize();
            Assert.IsNotNull(token);
        }

        [TestMethod, Ignore]
        public void BoardCard_Vault() {
            // TODO: for use with single use tokens
            string token = tokenizedCard.Tokenize();
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void Capture() {
            var response = track.Authorize(10m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1264")
                .WithClientTransactionId("137149")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var capture = response.Capture().Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
        }

        [TestMethod, Ignore]
        public void FindBoardedCard() {
            // TODO: This would need to be implemented
        }

        [TestMethod]
        public void UpdateBoardedCard() {
            tokenizedCard.ExpMonth = 12;
            tokenizedCard.ExpYear = 2025;

            bool success = tokenizedCard.UpdateTokenExpiry();
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ForceCapture_Keyed() {
            var response = card.Authorize(10m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("V00546")
                .WithInvoiceNumber("1559")
                .WithClientTransactionId("168901")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void ForceCapture_Encrypted_Keyed() {
            var response = encryptedCard.Authorize(10m)
                .WithCurrency("USD")
                .WithOfflineAuthCode("V00546")
                .WithInvoiceNumber("1559")
                .WithClientTransactionId("168901")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Refund_Keyed() {
            // TODO: There is some weirdness around adding the client transaction id to a refund by card.
            var response = card.Refund(4.01m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1701")
                //.WithClientTransactionId("165901")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void Refund_Encrypted_Keyed() {
            var response = encryptedCard.Refund(4.01m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1701")
                .WithClientTransactionId("165901")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Refund_TransactionId() {
            var response = card.Charge(4.01m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1703")
                .WithClientTransactionId("165902")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // TODO: ClientTransactionId not implemented on manage transactions (assumed same as original)
            var refund = response.Refund()
                .WithInvoiceNumber("1703")
                //.WithClientTransactionId("165902")
                .Execute();
            Assert.IsNotNull(refund);
            Assert.AreEqual("00", refund.ResponseCode);
        }

        [TestMethod]
        public void Refund_Swiped() {
            // TODO: There is some weirdness around adding the client transaction id to a refund by card.
            var response = track.Refund(4.01m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1701")
                //.WithClientTransactionId("165901")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Refund_Vault() {
            // TODO: There is some weirdness around adding the client transaction id to a refund by card.
            var response = tokenizedCard.Refund(1.83m)
                .WithCurrency("USD")
                .WithCashBack(0m)
                .WithConvenienceAmount(0m)
                //.WithTaxAmount(0m)
                .WithInvoiceNumber("1559")
                //PoNumber
                //CustomerCode
                //.WithClientTransactionId("166909")
                .WithAllowPartialAuth(false)
                .WithAllowDuplicates(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Sale_Keyed() {
            var response = card.Charge(1.05m)
                .WithCurrency("USD")
                .WithCashBack(0m)
                .WithConvenienceAmount(0m)
                //.WithTaxAmount(0m)
                .WithInvoiceNumber("12345")
                // PoNumber
                // CustomerCode
                .WithClientTransactionId("166901")
                .WithAllowPartialAuth(false)
                .WithAllowDuplicates(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void Sale_Encrypted_Keyed() {
            var response = encryptedCard.Charge(1.05m)
                .WithCurrency("USD")
                .WithCashBack(0m)
                .WithConvenienceAmount(0m)
                //.WithTaxAmount(0m)
                .WithInvoiceNumber("12345")
                // PoNumber
                // CustomerCode
                .WithClientTransactionId("166901")
                .WithAllowPartialAuth(false)
                .WithAllowDuplicates(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Sale_Swiped() {
            var response = track.Charge(1.29m)
                .WithCurrency("USD")
                .WithCashBack(0m)
                .WithConvenienceAmount(0m)
                //.WithTaxAmount(0m)
                .WithInvoiceNumber("12345")
                // PoNumber
                // CustomerCode
                .WithClientTransactionId("138401")
                .WithAllowPartialAuth(false)
                .WithAllowDuplicates(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Sale_Vault() {
            var response = tokenizedCard.Charge(1.29m)
                 .WithCurrency("USD")
                 .WithCashBack(0m)
                 .WithConvenienceAmount(0m)
                 //.WithTaxAmount(0m)
                 .WithInvoiceNumber("1559")
                 // PoNumber
                 // CustomerCode
                 .WithClientTransactionId("166909")
                 .WithAllowPartialAuth(false)
                 .WithAllowDuplicates(false)
                 .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Sale_Healthcare() {
            AutoSubstantiation autoSubstantiation = new AutoSubstantiation {
                CopaySubTotal = 60m
            };

            var response = card.Charge(202m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1556")
                // PoNumber
                // CustomerCode
                .WithClientTransactionId("166901")
                 .WithAllowPartialAuth(true)
                 .WithAllowDuplicates(false)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void Sale_Wallet() {
            var autoSubstantiation = new AutoSubstantiation {
                CopaySubTotal = 1m,
                ClinicSubTotal = 1m,
                DentalSubTotal = 1m,
                PrescriptionSubTotal = 1m,
                VisionSubTotal = 1m
            };

            var response = applePay.Charge()
                .WithCurrency("USD")
                .WithCashBack(1m)
                .WithConvenienceAmount(1.25m)
                //.WithTaxAmount(1m)
                .WithAutoSubstantiation(autoSubstantiation)
                .WithInvoiceNumber("INV123")
                // PoNumber
                // CustomerCode
                .WithClientTransactionId("TX123")
                 .WithAllowPartialAuth(true)
                 .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Sale_Level3() {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III) {
                Description = "Misc Goods",
                DiscountAmount = 1.01m,
                FreightAmount = 1.02m,
                DutyAmount = 1.03m,
                DestinationPostalCode = "06033",
                DestinationCountryCode = "840",
                OriginPostalCode = "01887",
                PoNumber = "17801",
                TaxAmount = 0m
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

            var response = card.Charge(1.05m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithInvoiceNumber("1556")
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void SettleBatch() {
            BatchSummary response = BatchService.CloseBatch();
            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.Status);
        }

        [TestMethod]
        public void UnboardCard() {
            string token = TestCards.MasterCardManual().Tokenize();
            Assert.IsNotNull(token);

            var deleteCard = new CreditCardData {
                Token = token
            };

            bool success = deleteCard.DeleteToken();
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Void() {
            var response = card.Charge(10m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }
    }
}
