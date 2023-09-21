using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Genius.Responses;
using GlobalPayments.Api.Terminals.Genius.ServiceConfigs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Environment = GlobalPayments.Api.Entities.Environment;

namespace GlobalPayments.Api.Tests.Terminals.Genius
{
    [TestClass]
    public class GeniusCreditTests {
        IDeviceInterface device;

        public GeniusCreditTests() {
            ConnectionConfig config = new ConnectionConfig();
            config.DeviceType = DeviceType.GENIUS_VERIFONE_P400;
            config.ConnectionMode = ConnectionModes.MEET_IN_THE_CLOUD;

            MitcConfig gatewayConfig = new MitcConfig(
                    "800000052964",
                    "80040205",
                    "LHgD5tP1KeUhIdTp1hW8gwiEliUdoUZz",
                    "u1cYb2xoGONWkGfSxp8js1BGgMOkO0tyMUP732qbAWM",
                    "uITbt4dHj0f6Q2EVDwuWWA9cGiDAQnyD",
                    "cedevice::at64t3"
            );

            config.GeniusMitcConfig = gatewayConfig;
            config.Environment = Environment.TEST;
            device = DeviceService.Create(config);


            Assert.IsNotNull(config);
        }

        [TestMethod]
        public void TestSale() {

            AutoSubstantiation auto = new AutoSubstantiation();
            auto.RealTimeSubstantiation =true;
            auto.DentalSubTotal = 5.00m;
            auto.ClinicSubTotal = 5.00m;
            auto.VisionSubTotal = 5.00m;
            auto.CopaySubTotal = 5.00m;

            Address address = new Address();
            address.PostalCode= "84042";

            ITerminalResponse response = device.CreditSale(2526)
                   .WithClientTransactionId(getRandomNumber(6))
                   .WithInvoiceNumber(getRandomNumber(8))
                   .WithAutoSubstantiation(auto)
                   .WithAddress(address)
                   .WithAllowPartialAuth(true)
                   .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        [TestMethod]
        public void TestRefundPrevSale()
        {
            var clientTransId = "mapsToReference_id" + getRandomNumber(6);

            ITerminalResponse saleResponse = device.CreditSale(100.00m)
                          .WithClientTransactionId(clientTransId)
                          .WithInvoiceNumber(getRandomNumber(8))
                          .Execute();

        
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            ITerminalResponse refundResponse = device.RefundById(100.00m)
               .WithClientTransactionId(clientTransId)
               .Execute();

            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);

        }
        [TestMethod]
        public void TestIndependentRefund()
        {
            var clientTransId = "mapsToReference_id" + getRandomNumber(6);

            ITerminalResponse refundResponse = device.CreditRefund(100.00m)
                          .WithClientTransactionId(clientTransId)
                          .WithInvoiceNumber(getRandomNumber(8))
                          .Execute();

            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);       
        }
        [TestMethod]
        public void TestVoidPrevSale()
        {
            var clientTransId = "mapsToReference_id" + getRandomNumber(6);

            ITerminalResponse saleResponse = device.CreditSale(100.00m)
                          .WithClientTransactionId(clientTransId)
                          .WithInvoiceNumber(getRandomNumber(8))
                          .Execute();

            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            ITerminalResponse refundResponse = device.CreditVoid()
                          .WithClientTransactionId(clientTransId)                          
                          .Execute();

            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);
        }
        [TestMethod]
        public void TestVoidPrevDebitSale()
        {
            var clientTransId = "mapsToReference_id" + getRandomNumber(6);

            ITerminalResponse debitSaleResponse = device.DebitSale(100.00m)
                          .WithClientTransactionId(clientTransId)
                          .WithInvoiceNumber(getRandomNumber(8))
                          .Execute();

            Assert.IsNotNull(debitSaleResponse);
            Assert.AreEqual("00", debitSaleResponse.ResponseCode);            
            ITerminalResponse voidResponse = device.DebitVoid()
                          .WithClientTransactionId(clientTransId)
                          .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }
        [TestMethod]
        public void TestVoidPrevRefund()
        {
            var clientTransId = "mapsToReference_id" + getRandomNumber(6);

            ITerminalResponse response = device.CreditRefund(100.00m)
                          .WithClientTransactionId(clientTransId)
                          .WithInvoiceNumber(getRandomNumber(8))
                          .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);


            ITerminalResponse voidResponse = device.VoidRefund()
                          .WithClientTransactionId(clientTransId)
                          .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }
        [TestMethod]
        public void TestSaleReport()
        {
            var clientTransId = "mapsToReference_id" + getRandomNumber(6);

            var saleResponse = device.CreditSale(100.00m)
                          .WithClientTransactionId(clientTransId)
                          .WithInvoiceNumber(getRandomNumber(8))
                          .Execute();

            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
            var transactionReport = device.GetTransactionDetails(TransactionType.Sale, clientTransId, TransactionIdType.CLIENT_TRANSACTION_ID)
                                                .Execute() as MitcResponse;                            

            Assert.IsNotNull(transactionReport);
            Assert.AreEqual("00", transactionReport.ResponseCode);

        }
        [TestMethod]
        public void TestRefundReport()
        {
            var clientTransId = "mapsToReference_id" + getRandomNumber(6);

            var response = device.CreditRefund(100.00m)
                          .WithClientTransactionId(clientTransId)
                          .WithInvoiceNumber(getRandomNumber(8))
                          .Execute();


            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var transactionReport = device.GetTransactionDetails(TransactionType.Refund, clientTransId, TransactionIdType.CLIENT_TRANSACTION_ID)
                                                .Execute() as MitcResponse;

            Assert.IsNotNull(transactionReport);
            Assert.AreEqual("00", transactionReport.ResponseCode);
        }
        static string getRandomNumber(int length)
        {
            Random generator = new Random();
            string r = generator.Next(0, 1000000).ToString("D6");
            return r;
        }

    }
}
