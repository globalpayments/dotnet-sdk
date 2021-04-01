using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.Terminals;
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
                .WithCardBrandStorage(StoredCredentialInitiator.CardHolder, response.CardBrandTransactionId)
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
            var response = card.Charge(15m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
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
                .WithClientTransactionId("123456")
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

        #region Step 4: LEVEL 3 Enhanced Data Tests

        [TestMethod]
        public void Visa_Level3_Sale() {

            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III) {
                PoNumber = "1784951399984509620",
                TaxAmount = 0.01m,
                DestinationPostalCode = "85212",
                DestinationCountryCode = "USA",
                OriginPostalCode = "22193",
                SummaryCommodityCode = "SCC",
                VAT_InvoiceNumber = "UVATREF162",
                OrderDate = DateTime.Now,
                FreightAmount = 0.01m,
                DutyAmount = 0.01m,
                AdditionalTaxDetails = new AdditionalTaxDetails {
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m
                }
            };
            commercialData.AddLineItems(
                new CommercialLineItem {
                    ProductCode = "PRDCD1",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 1 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails {
                        DiscountAmount = 0.50m
                    }
                },
                new CommercialLineItem {
                    ProductCode = "PRDCD2",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 2 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails {
                        DiscountAmount = 0.50m
                    }
                }
            );

            var response = VisaManual.Charge(0.53m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var edit = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void MasterCard_Level3_Sale() {

            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III) {
                PoNumber = "9876543210",
                TaxAmount = 0.01m,
                DestinationPostalCode = "85212",
                DestinationCountryCode = "USA",
                OriginPostalCode = "22193",
                SummaryCommodityCode = "SCC",
                VAT_InvoiceNumber = "UVATREF162",
                OrderDate = DateTime.Now,
                FreightAmount = 0.01m,
                DutyAmount = 0.01m,
                AdditionalTaxDetails = new AdditionalTaxDetails {
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m,
                }
            };
            commercialData.AddLineItems(
                new CommercialLineItem {
                    ProductCode = "PRDCD1",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 1 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails {
                        DiscountAmount = 0.50m
                    }
                }
            );

            var response = MasterCardManual.Charge(0.53m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var edit = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        #endregion
    }
}
