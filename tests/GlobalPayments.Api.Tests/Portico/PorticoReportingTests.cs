using System;
using System.Collections.Generic;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.Terminals;
using GlobalPayments.Api.Utils.Logging;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoReportingTests {

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
                SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w",
                IsSafDataSupported = true,
                RequestLogger = new RequestConsoleLogger()
            });

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

            VisaManual = new CreditCardData
            {
                Number = "4012000098765439",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvn = "999"
            };
        }
        [TestMethod]
        public void ReportActivity()
        {
            List<TransactionSummary> summary = ReportingService.Activity()
                .WithTimeZoneConversion(TimeZoneConversion.UTC)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-2))
                .And(SearchCriteria.EndDate, DateTime.UtcNow)
                .Execute();
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.Count > 0);
        }
        [TestMethod]
        public void ReportTransactionDetailGetShippingDate()
        {
            EcommerceInfo ecommerceInfo = new EcommerceInfo();
            ecommerceInfo.ShipDay = 5;
            ecommerceInfo.ShipMonth = 6;
            //Do auth
            var authResponse = VisaManual.Charge(10m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .WithEcommerceInfo(ecommerceInfo)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            TransactionSummary response = ReportingService.TransactionDetail(authResponse.TransactionId).Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is TransactionSummary);
            Assert.IsTrue(response.ShippingDay > 0);
            Assert.IsTrue(response.ShippingMonth > 0);
        }
        [TestMethod]
        public void ReportTransactionDetail() 
        {
            //Do auth
            var authResponse = VisaManual.Charge(10m)
                .WithCurrency("USD")
                .WithClientTransactionId(ClientTransactionId)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            TransactionSummary response = ReportingService.TransactionDetail(authResponse.TransactionId).Execute();
            Assert.IsNotNull(response);
            Assert.IsTrue(response is TransactionSummary);
        }
        [TestMethod]
        public void ReportFindTransactionWithCriteria()
        {
            List<TransactionSummary> summary = ReportingService.FindTransactions()
                .WithTimeZoneConversion(TimeZoneConversion.Merchant)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-30))
                .And(SearchCriteria.EndDate, DateTime.UtcNow)
                .And(SearchCriteria.SAFIndicator, "Y")
                .And(SearchCriteria.BatchId, "992515")
                .Execute();

            Assert.IsNotNull(summary);
        }
        [TestMethod]
        public void ReportFindTransactionWithTransactionId() {
            List<TransactionSummary> summary = ReportingService.FindTransactions("2045823572").Execute();
            Assert.IsNotNull(summary);
        }
        [TestMethod]
        public void FindTransByDate()
        {
            var items = ReportingService.FindTransactions()
                .WithTimeZoneConversion(TimeZoneConversion.Merchant)
                .WithStartDate(DateTime.Today.AddDays(-10))
                .WithEndDate(DateTime.Today)
                .Execute();
            Assert.IsNotNull(items);
            var item = ReportingService.TransactionDetail(items[0].TransactionId)
                .Execute();
            Assert.IsNotNull(item);
        }
        [TestMethod]
        public void ReportFindTransactionNoCriteria() {
            List<TransactionSummary> summary = ReportingService.FindTransactions().Execute();
            Assert.IsNotNull(summary);
        }
        [TestMethod]
        public void ReportActivityWithNewCryptoURL() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });
          
            List<TransactionSummary> summary = ReportingService.Activity()
                .WithTimeZoneConversion(TimeZoneConversion.Merchant)
                .Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-2))
                .And(SearchCriteria.EndDate, DateTime.UtcNow)
                .Execute();

            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.Count > 0);
        }
        [TestMethod]
        public void Visa_Level3_DetailsReport()
        {

            var commercialData = new CommercialData(TaxType.SALESTAX, CommercialIndicator.Level_III)
            {
                PoNumber = "1784951399984509620",
                TaxAmount = 0.09m,
                DestinationPostalCode = "85212",
                DestinationCountryCode = "USA",
                OriginPostalCode = "22193",
                SummaryCommodityCode = "SCC",
                VAT_InvoiceNumber = "UVATREF162",
                OrderDate = DateTime.Now,
                FreightAmount = 0.01m,
                DutyAmount = 0.01m,
                VATTaxAmtFreight = .02m,
                VATTaxRateFreight = .04m,
                AdditionalTaxDetails = new AdditionalTaxDetails
                {
                    TaxAmount = 0.03m,
                    TaxRate = 0.05m
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
                .WithInvoiceNumber("1234")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var edit = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);

            TransactionSummary details = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(details);
            Assert.IsTrue(details is TransactionSummary);
            Assert.AreEqual(details.CommercialData.LineItems[0].Description, "PRODUCT 1 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[0].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[0].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[0].ProductCode, "PRDCD1");
            Assert.AreEqual(details.CommercialData.LineItems[1].Description, "PRODUCT 2 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[1].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[1].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[1].ProductCode, "PRDCD2");
            Assert.IsTrue(details.MerchantNumber.Length > 0);
            Assert.AreEqual(details.InvoiceNumber, "1234");
            Assert.AreEqual(details.CommercialData.DutyAmount, .01m);
            Assert.AreEqual(details.CommercialData.FreightAmount, .01m);
            Assert.AreEqual(details.TaxAmount, .09m);
            Assert.IsTrue(details.CommercialData.OrderDate < DateTime.Now);
            Assert.IsTrue(details.CommercialData.OrderDate > DateTime.Now.AddDays(-1));
            Assert.AreEqual(details.CommercialData.DestinationPostalCode, "85212");
            Assert.AreEqual(details.CommercialData.PoNumber, "1784951399984509620");
            Assert.AreEqual(details.CommercialData.DestinationCountryCode, "USA");
            Assert.AreEqual(details.CommercialData.OriginPostalCode, "22193");
            Assert.AreEqual(details.CommercialData.SummaryCommodityCode.Trim(), "SCC");
            Assert.AreEqual(details.CommercialData.VAT_InvoiceNumber, "UVATREF162");
            Assert.IsTrue(details.CommercialData.OrderDate < DateTime.Now);


        }
        [TestMethod]
        public void Visa_Level3_BusCard_Auth_DetailsReport()
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


            TransactionSummary details = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(details);
            Assert.IsTrue(details is TransactionSummary);
            Assert.AreEqual(details.CommercialData.LineItems[0].Description, "PRODUCT 1 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[0].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[0].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[0].ProductCode, "PRDCD1");
            Assert.AreEqual(details.CommercialData.LineItems[1].Description, "PRODUCT 2 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[1].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[1].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[1].ProductCode, "PRDCD2");


            Assert.AreEqual(details.CommercialData.DutyAmount, .01m);
            Assert.AreEqual(details.CommercialData.FreightAmount, .01m);
            Assert.AreEqual(details.TaxAmount, .01m);
            Assert.AreEqual(details.CommercialData.DestinationPostalCode, "85212");
            Assert.AreEqual(details.CommercialData.PoNumber, "1784951399984509620");
            Assert.AreEqual(details.CommercialData.DestinationCountryCode, "USA");
            Assert.AreEqual(details.CommercialData.OriginPostalCode, "22193");
            Assert.AreEqual(details.CommercialData.SummaryCommodityCode.Trim(), "SCC");
            Assert.AreEqual(details.CommercialData.VAT_InvoiceNumber, "UVATREF162");
            Assert.IsTrue(details.CommercialData.OrderDate < DateTime.Now);

        }
        [TestMethod]
        public void Visa_Level3_PurchCard_Auth_DetailsReport()
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

            TransactionSummary details = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(details);
            Assert.IsTrue(details is TransactionSummary);
            Assert.AreEqual(details.CommercialData.LineItems[0].Description, "PRODUCT 1 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[0].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[0].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[0].ProductCode, "PRDCD1");
            Assert.AreEqual(details.CommercialData.LineItems[1].Description, "PRODUCT 2 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[1].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[1].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[1].ProductCode, "PRDCD2");


            Assert.AreEqual(details.CommercialData.DutyAmount, .01m);
            Assert.AreEqual(details.CommercialData.FreightAmount, .01m);
            Assert.AreEqual(details.TaxAmount, .01m);
            Assert.AreEqual(details.CommercialData.DestinationPostalCode, "85212");
            Assert.AreEqual(details.CommercialData.PoNumber, "1784951399984509620");
            Assert.AreEqual(details.CommercialData.DestinationCountryCode, "USA");
            Assert.AreEqual(details.CommercialData.OriginPostalCode, "22193");
            Assert.AreEqual(details.CommercialData.SummaryCommodityCode.Trim(), "SCC");
            Assert.AreEqual(details.CommercialData.VAT_InvoiceNumber, "UVATREF162");
            Assert.IsTrue(details.CommercialData.OrderDate < DateTime.Now);
            Assert.IsTrue(details.TaxType.ToUpper().Contains("SALESTAX"));
        }
        [TestMethod]
        public void Visa_Level3_CorpCard_Auth_DetailsReport()
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
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 20.00m
                    }
                },
                new CommercialLineItem
                {
                    ProductCode = "PRDCD2",
                    UnitCost = 0.01m,
                    Quantity = 1m,
                    UnitOfMeasure = "METER",
                    Description = "PRODUCT 2 NOTES",
                    DiscountDetails = new DiscountDetails
                    {
                        DiscountAmount = 20.00m
                    }
                }
            );

            var response = VisaManual.Authorize(123.45m)
                .WithCurrency("USD")
                .WithCommercialRequest(true)
                .WithClientTransactionId("PF123456789")
                .WithAllowDuplicates(true)
                .WithAddress(Address)
                .WithSurchargeAmount(5m)
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

            TransactionSummary details = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(details);
            Assert.IsTrue(details is TransactionSummary);
            Assert.AreEqual(details.CommercialData.LineItems[0].Description, "PRODUCT 1 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[0].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[0].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[0].ProductCode, "PRDCD1");
            Assert.AreEqual(details.CommercialData.LineItems[0].DiscountDetails.DiscountAmount, 20m);
            Assert.AreEqual(details.CommercialData.LineItems[1].Description, "PRODUCT 2 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[1].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[1].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[1].ProductCode, "PRDCD2");
            Assert.AreEqual(details.CommercialData.LineItems[1].DiscountDetails.DiscountAmount, 20m);
            Assert.AreEqual(details.SurchargeAmount, 5m);
            Assert.AreEqual(details.CommercialData.DutyAmount, .01m);
            Assert.AreEqual(details.CommercialData.FreightAmount, .01m);
            Assert.AreEqual(details.TaxAmount, .01m);
            Assert.AreEqual(details.Currency, "USD");
            Assert.AreEqual(details.CommercialData.DestinationPostalCode, "85212");
            Assert.AreEqual(details.CommercialData.PoNumber, "1784951399984509620");
            Assert.AreEqual(details.CommercialData.DestinationCountryCode, "USA");
            Assert.AreEqual(details.CommercialData.OriginPostalCode, "22193");
            Assert.AreEqual(details.CommercialData.SummaryCommodityCode.Trim(), "SCC");
            Assert.AreEqual(details.CommercialData.VAT_InvoiceNumber, "UVATREF162");
            Assert.IsTrue(details.CommercialData.OrderDate < DateTime.Now);
            Assert.IsTrue(details.TaxType.ToUpper().Contains("SALESTAX"));
        }
        [TestMethod]
        public void MasterCard_Level3_Sale_DetailsReport()
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
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var edit = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(edit);
            Assert.AreEqual("00", edit.ResponseCode);


            TransactionSummary details = ReportingService.TransactionDetail(response.TransactionId).Execute();
            Assert.IsNotNull(details);
            Assert.IsTrue(details is TransactionSummary);
            Assert.AreEqual(details.CommercialData.LineItems[0].Description, "PRODUCT 1 NOTES");
            Assert.AreEqual(details.CommercialData.LineItems[0].Quantity, 1m);
            Assert.AreEqual(details.CommercialData.LineItems[0].UnitOfMeasure, "METER");
            Assert.AreEqual(details.CommercialData.LineItems[0].ProductCode, "PRDCD1");
            Assert.AreEqual(details.TaxAmount, .01m);
            Assert.AreEqual(details.CommercialData.PoNumber, "9876543210");
            Assert.IsTrue(details.TaxType.ToUpper().Contains("SALESTAX"));
        }

        [TestMethod]
        public void ValidateResponseDateAndTransactionDateIsMappedInTransactionSummary()
        {            
            var responseMasterCard = MasterCardManual.Charge(17)
               .WithCurrency("USD")
               .WithAddress(Address)
               .WithAllowDuplicates(true)
               .Execute();
                Assert.IsNotNull(responseMasterCard);
                Assert.AreEqual("00", responseMasterCard.ResponseCode);

            var responseVisa = VisaManual.Charge(23)
               .WithCurrency("USD")
               .WithAddress(Address)
               .WithAllowDuplicates(true)
               .Execute();
                Assert.IsNotNull(responseVisa);
                Assert.AreEqual("00", responseVisa.ResponseCode);

            var items = ReportingService.FindTransactions()
               .WithStartDate(DateTime.Today.AddDays(-1))
               .WithEndDate(DateTime.Today)
               .Execute();

            Assert.IsNotNull(items[0].ResponseDate);
            Assert.IsNotNull(items[0].TransactionDate);

            var item = ReportingService.TransactionDetail(items[0].TransactionId)
                .Execute();

            Assert.IsNotNull (item.ResponseDate);
            Assert.IsNotNull(item.TransactionDate);
        }

        [TestMethod]
        public void ReportBatchDetail_WithClientTxnId_and_BatchID()
        {
            var clientTxnId = ClientTransactionId;
            //Do auth
            var authResponse = VisaManual.Authorize(10m)
                .WithCurrency("USD")                
                .WithClientTransactionId(clientTxnId)               
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            //Do a capture to add to batch
            Transaction captureResponse = authResponse.Capture(10m)
                                                       .WithGratuity(2m)
                                                       .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);

            List<TransactionSummary> details = ReportingService.BatchDetail()
                                                                .WithBatchId("992515")
                                                                .Execute();

            //Get reportItem that matches the clienttxnid passed initally
            TransactionSummary reportItem = details.Find(x => x.ClientTransactionId == clientTxnId);
            Assert.IsNotNull(reportItem);            
            Assert.AreEqual(reportItem.ClientTransactionId, clientTxnId);            
        }

        [TestMethod]
        public void ReportOpenAuths_WithClientTxnId()
        {
            var clientTxnId = ClientTransactionId;
            //Do auth
            var authResponse = VisaManual.Authorize(10m)
                .WithCurrency("USD")
                .WithClientTransactionId(clientTxnId)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);           

            List<TransactionSummary> details = ReportingService.OpenAuths()                                                                
                                                                .Execute();

            //Get reportItem that matches the clienttxnid passed initally
            TransactionSummary reportItem = details.Find(x => x.ClientTransactionId == clientTxnId);
            Assert.IsNotNull(reportItem);
            Assert.AreEqual(reportItem.ClientTransactionId, clientTxnId);
        }

        [TestMethod]
        public void FindTransactions() {
            var response = ReportingService.FindTransactions()
                .WithStartDate(DateTime.Today.AddDays(-10))
                .WithEndDate(DateTime.Today)
                //.Where(SearchCriteria.StartDate, DateTime.UtcNow.AddDays(-2))
                //.And(SearchCriteria.EndDate, DateTime.UtcNow)                
                .Execute();
            Assert.IsNotNull(response);
        }
    }
}
