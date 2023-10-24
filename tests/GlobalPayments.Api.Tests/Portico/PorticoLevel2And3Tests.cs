using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests
{
    [TestClass]
    public class PorticoLevel2And3Tests
    {
        private IncrementalNumberProvider _generator;

        CreditCardData card;
        CreditTrackData track;
        CreditCardData VisaManual { get; set; }
        CreditCardData MasterCardManual { get; set; }
        CreditCardData MasterCardVerifyCorpCard { get; set; }
        CreditCardData MasterCardVerifyNonCorpCard { get; set; }
        CreditCardData MasterCardVerifyNonCommercialCard { get; set; }
        Address Address { get; set; }
        string ClientTransactionId
        {
            get
            {
                if (_generator == null)
                {
                    _generator = IncrementalNumberProvider.GetInstance();
                }
                return _generator.GetRequestId().ToString();
            }
        }

        [TestInitialize]
        public void Init()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w"
            });

            card = new CreditCardData
            {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardHolderName = " UnitTest "
            };

            track = new CreditTrackData
            {
                Value = "<E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|>;",
                EncryptionData = new EncryptionData
                {
                    Version = "01"
                }
            };

            Address = new Address
            {
                StreetAddress1 = "8320",
                PostalCode = "85284"
            };

            MasterCardManual = new CreditCardData
            {
                Number = "5146315000000055",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "998"
            };
            MasterCardVerifyCorpCard = new CreditCardData
            {
                Number = "5378190497136574",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "998"
            };
            MasterCardVerifyNonCorpCard = new CreditCardData
            {
                Number = "5305740490962141",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "998"
            };
            
            VisaManual = new CreditCardData
            {
                Number = "4012000098765439",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "999"
            };
        }

        #region LEVEL 3 Enhanced Data Tests

        [TestMethod]
        public void Visa_Level3_Sale()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III)
            {
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
                AdditionalTaxDetails = new AdditionalTaxDetails
                {
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m
                }
            };
            commercialData.AddLineItems(
                new CommercialLineItem
                {
                    ProductCode = "PRDCD1",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 1 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 0.50m
                    }
                },
                new CommercialLineItem
                {
                    ProductCode = "PRDCD2",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 2 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails
                    {
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
        public void Visa_Level3_BusCard_Auth()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III)
            {
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
                AdditionalTaxDetails = new AdditionalTaxDetails
                {
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m
                }
            };
            commercialData.AddLineItems(
                new CommercialLineItem
                {
                    ProductCode = "PRDCD1",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 1 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 0.50m
                    }
                },
                new CommercialLineItem
                {
                    ProductCode = "PRDCD2",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 2 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 0.50m
                    }
                }
            );

            var response = VisaManual.Authorize(112.34m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId("PF123456789")
                .WithAllowDuplicates(true)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionId = response.TransactionId;
            Enum.TryParse(response.CardType, out CardType cardType);

            var CpcIndicator = response.CommercialIndicator;
            Assert.AreEqual("B", CpcIndicator);
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)

            var edit = Transaction.FromId(transactionId, cardType)
                .Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            var captureResponse = Transaction.FromId(transactionId)
                .Capture(112.34m)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }
        [TestMethod]
        public void Visa_Level2_BusCard_Sale()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_II)
            {
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
                AdditionalTaxDetails = new AdditionalTaxDetails
                {
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m
                }
            };
            commercialData.AddLineItems(
                new CommercialLineItem
                {
                    ProductCode = "PRDCD1",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 1 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    TotalAmount = .01m,
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 0.50m
                    }
                });
            var response = VisaManual.Charge(112.34m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId("PF123456789")
                .WithAllowDuplicates(true)
                .WithAddress(Address)
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionId = response.TransactionId;
            Enum.TryParse(response.CardType, out CardType cardType);

            var CpcIndicator = response.CommercialIndicator;
            Assert.AreEqual("B", CpcIndicator);
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)
        }
        [TestMethod]
        public void Visa_Level3_PurchCard_Auth()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III)
            {
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
                AdditionalTaxDetails = new AdditionalTaxDetails
                {
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m
                }
            };
            commercialData.AddLineItems(
                new CommercialLineItem
                {
                    ProductCode = "PRDCD1",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 1 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    //TotalAmount = 0.01m,
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 0.50m
                    }
                },
                new CommercialLineItem
                {
                    ProductCode = "PRDCD2",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    //TotalAmount = 0.01m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 2 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 0.50m
                    }
                }
            );

            var response = VisaManual.Authorize(134.56m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId("PF123456789")
                .WithAllowDuplicates(true)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionId = response.TransactionId;
            Enum.TryParse(response.CardType, out CardType cardType);

            var CpcIndicator = response.CommercialIndicator;
            Assert.AreEqual("S", CpcIndicator);
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)

            var edit = Transaction.FromId(transactionId, cardType)
                .Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            var captureResponse = Transaction.FromId(transactionId)
                .Capture(134.56m)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }
        [TestMethod]
        public void Visa_Level3_CorpCard_Auth()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III)
            {
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
                AdditionalTaxDetails = new AdditionalTaxDetails
                {
                    TaxAmount = 0.01m,
                    TaxRate = 0.04m
                }
            };
            commercialData.AddLineItems(
                new CommercialLineItem
                {
                    ProductCode = "PRDCD1",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 1 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 0.50m
                    }
                },
                new CommercialLineItem
                {
                    ProductCode = "PRDCD2",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 2 NOTES",
                    // CommodityCode = "12DIGIT ACCO",
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 0.50m
                    }
                }
            );

            var response = VisaManual.Authorize(123.45m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId("PF123456789")
                .WithAllowDuplicates(true)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionId = response.TransactionId;
            Enum.TryParse(response.CardType, out CardType cardType);

            var CpcIndicator = response.CommercialIndicator;
            Assert.AreEqual("R", CpcIndicator);
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)

            var edit = Transaction.FromId(transactionId, cardType)
                .Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            var captureResponse = Transaction.FromId(transactionId)
                .Capture(123.45m)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }
        [TestMethod]
        public void MasterCard_Level3_Sale()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III)
            {
                PoNumber = "9876543210",
                TaxAmount = 0.01m,
            };
            commercialData.AddLineItems(
                new CommercialLineItem
                {
                    Description = "PRODUCT 1 NOTES",
                    ProductCode = "PRDCD1",
                    Quantity = 1m,
                    TotalAmount = 0.01m,
                    UnitOfMeasure = "METER",

                }
            );

            var response = MasterCardManual.Charge(0.53m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId(ClientTransactionId)
                .WithAddress(Address)
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            TransactionSummary details = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(details);
            Assert.IsTrue(details is TransactionSummary);
            Assert.AreEqual(details.PoNumber, "9876543210");
            Assert.IsTrue(details.TaxType.ToUpper().Contains("SALESTAX"));

            commercialData.PoNumber = "1234";
            var edit = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);
        }

        [TestMethod]
        public void MasterCard_Level3_Auth()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III)
            {
                PoNumber = "9876543210",
                TaxAmount = 0.01m,
            };
            commercialData.AddLineItems(
                new CommercialLineItem
                {
                    Description = "PRODUCT 1 NOTES",
                    ProductCode = "PRDCD1",
                    Quantity = 1m,
                    TotalAmount = 0.01m,
                }
            );

            var response = MasterCardManual.Authorize(111.09m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId(ClientTransactionId)
                .WithCommercialData(commercialData)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionId = response.TransactionId;
            Enum.TryParse(response.CardType, out CardType cardType);

            var CpcIndicator = response.CommercialIndicator;
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)
            
            if (CpcIndicator.IndexOfAny(new char[] { 'B', 'R', 'S', 'L' }) != -1 && commercialData.LineItems.Count>0)
            {
                var transaction = Transaction.FromId(transactionId);
                transaction.CardType = cardType.ToString();
                var edit = transaction.Edit()
                    .WithCommercialData(commercialData)
                    .Execute();

                Assert.IsNotNull(edit);
                Assert.AreEqual("00", edit.ResponseCode);
            }

            var captureResponse = Transaction.FromId(transactionId)
                .Capture(111.09m)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }
        [TestMethod]
        public void MasterCard_Level2_Auth()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_II)
            {
                PoNumber = "9876543210",
                TaxAmount = 0.01m,
            };

            var response = MasterCardManual.Authorize(111.09m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId(ClientTransactionId)
                .WithCommercialData(commercialData)
                .WithAddress(Address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionId = response.TransactionId;
            Enum.TryParse(response.CardType, out CardType cardType);

            var CpcIndicator = response.CommercialIndicator;
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)
 
            var captureResponse = Transaction.FromId(transactionId)
                .Capture(111.09m)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }
        [TestMethod]
        public void MasterCard_Level3_PurchCard_Auth()
        {
            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III)
            {
                PoNumber = "9876543210",
                TaxAmount = 0.01m,
            };
            commercialData.AddLineItems(
                new CommercialLineItem
                {
                    Description = "PRODUCT 1 NOTES",
                    ProductCode = "PRDCD1",
                    Quantity = 1m,
                    TotalAmount = 0.01m,
                    UnitOfMeasure = "METER",

                }
            );

            var response = MasterCardManual.Authorize(111.07m)
                            .WithCurrency("USD")
                            .WithCommercialRequest(true)
                            .WithClientTransactionId(ClientTransactionId)
                            .WithAddress(Address)
                            .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionId = response.TransactionId;
            Enum.TryParse(response.CardType, out CardType cardType);

            var CpcIndicator = response.CommercialIndicator;
            Assert.AreEqual("S", CpcIndicator);
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)

            var edit = Transaction.FromId(transactionId, cardType)
                .Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            var captureResponse = Transaction.FromId(transactionId)
                .Capture(111.07m)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }
        [TestMethod]
        public void MasterCard_Level3_VerifyCorpCard_Verify()
        {
            var response = MasterCardVerifyCorpCard.Verify()
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var CpcIndicator = response.CommercialIndicator;
            Assert.AreEqual("R", CpcIndicator);
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)
        }
        [TestMethod]
        public void MasterCard_Level3_VerifyNonCorpCard_Verify()
        {
            var response = MasterCardVerifyNonCorpCard.Verify()
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var CpcIndicator = response.CommercialIndicator;
            Assert.AreEqual("0", CpcIndicator);
            // B(Business Card)
            // R(Corporate Card)
            // S(Purchasing Card)
            // L(B2B - Settlement amount may not exceed Authorized amount)
        }
        #endregion
    }
}
