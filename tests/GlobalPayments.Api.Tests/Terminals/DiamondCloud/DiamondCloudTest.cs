using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Diamond.Entities.Enums;
using GlobalPayments.Api.Terminals.Diamond.Responses;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.DiamondCloud {
    [TestClass]
    public class DiamondCloudTest {
        IDeviceInterface _device;
        RandomIdProvider _requestidProvider;
        private string posID = "1342641186174645";

        public DiamondCloudTest() {
            _requestidProvider = new RandomIdProvider();

            _device = DeviceService.Create(new DiamondCloudConfig
            {
                DeviceType = DeviceType.PAX_A920,
                ConnectionMode = ConnectionModes.DIAMOND_CLOUD,
                RequestIdProvider = _requestidProvider,
                Timeout = 15,
                IsvID = "154F070E3E474AB98B00D73ED81AAA93",
                SecretKey = "8003672638",
                Region = Region.EU.ToString(),
                PosID = posID
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void CreditSale() {
            ITerminalResponse response = _device.Sale(2m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void GetTransactionStatus() {
            ITerminalResponse response = _device.LocalDetailReport()
                .Where(DiamondCloudSearchCriteria.ReferenceNumber, "ZYMD938VKW4")
                .Execute() as DiamondCloudResponse;

            Assert.AreEqual("sale", response.Command);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("ACCEPTED", response.ResponseCode);
        }

        [TestMethod]
        public void CreditVoid() {
            ITerminalResponse response = _device.Void()
                .WithTransactionId("KQM5X7W8BQX")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void CreditReturn() {
            ITerminalResponse response = _device.Refund(1)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void LinkedRefund() {
            ITerminalResponse response = _device.Authorize(2.01m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);

            string transactionId = response.TransactionId;

            Thread.Sleep(15000);

            ITerminalResponse refund = _device.RefundById()
                .WithTransactionId(transactionId)
                .Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual("00", refund.DeviceResponseCode);
            Assert.IsNotNull(refund.TransactionId);
        }

        [TestMethod]
        public void TipAdjust() {
            ITerminalResponse response = _device.TipAdjust(1.01m)
                .WithAmount(5.01m)
                .WithTransactionId("M3QR97ZDM8M")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void Authorize() {
            ITerminalResponse response = _device.Authorize(2m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void Capture() {
            ITerminalResponse response = _device.Capture(2m)
                .WithTransactionId("43M79ABJEWV")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void AuthCapture() {
            ITerminalResponse response = _device.Authorize(2m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
            
            Thread.Sleep(15000);
            
            ITerminalResponse capture = _device.Capture(2m)
                .WithTransactionId(response.TransactionId)
                .Execute();

            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.DeviceResponseCode);
            Assert.IsNotNull(capture.TransactionId);
        }

        [TestMethod]
        public void CancelAuth() {
            ITerminalResponse response = _device.DeletePreAuth()
                .WithTransactionId("DJKK7BY4MWV")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void AuthIncreasing() {
            ITerminalResponse response = _device.IncreasePreAuth(3)
                .WithTransactionId("DJKK7BY4MWV")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void EbtPurchase() {
            SetUSConfig();
            ITerminalResponse response = _device.Sale(1m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void EbtBalance() {
            SetUSConfig();
            ITerminalResponse response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void EbtReturn() {
            SetUSConfig();
            ITerminalResponse response = _device.Refund(5.02m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void GiftBalance() {
            SetUSConfig();
            //Transaction type Balance for payment type not supported in EU
            ITerminalResponse response = _device.Balance()
                .WithCurrency(CurrencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void GiftReload() {
            SetUSConfig();
            ITerminalResponse response = _device.AddValue(1m)
                .WithCurrency(CurrencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void GiftRedeem() {
            SetUSConfig();
            ITerminalResponse response = _device.Sale(1m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithCurrency(CurrencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void BatchClose() {
            ITerminalResponse response = _device.BatchClose() as ITerminalResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.IsNotNull(response.TransactionId);
        }

        [TestMethod]
        public void SaleStatusUrlResponse() {
            DeviceCloudService service = new DeviceCloudService(new DiamondCloudConfig {
                DeviceType = DeviceType.PAX_A920,
                ConnectionMode = ConnectionModes.DIAMOND_CLOUD,
                RequestIdProvider = _requestidProvider,
                Timeout = 15,
                IsvID = "154F070E3E474AB98B00D73ED81AAA93",
                SecretKey = "8003672638",
                Region = Region.EU.ToString(),
                PosID = posID
            });
            string responseJson = "{\"IsvId\":\"154F070E3E474AB98B00D73ED81AAA93\",\"InvoiceId\":\"\",\"CloudTxnId\":\"XJ98K73YJ9N\",\"traceId\":\"\",\"followId\":\"\",\"Device\":\"1850747855_2\",\"PosId\":\"1342641186174645\",\"PaymentResponse\":{ \"PaymentResponse\":{ \"aosa\":null,\"applicationVersion\":\"1.6.2\",\"authorizationCode\":null,\"authorizationMessage\":\"000023\",\"authorizationMethod\":\"?\",\"authorizationType\":\"?\",\"cardBrandName\":\"MC CREDIT\",\"cardSource\":\"P\",\"cashbackAmount\":\"500\",\"currencyExchangeRate\":null,\"date\":\"2023.09.11\",\"dccCurrencyExponent\":null,\"dccText1\":null,\"dccText2\":null,\"errorMessage\":null,\"maskedCardNumber\":\"************0036\",\"merchantId\":\"888880000000373\",\"result\":\"1\",\"serverMessage\":null,\"slipNumber\":\"23\",\"terminalCurrency\":null,\"terminalId\":\"66677768\",\"terminalPrintingIndicator\":\"1\",\"time\":\"15:37:55\",\"tipAmount\":null,\"token\":null,\"transactionAmount\":\"1000\",\"transactionAmountInTerminalCurrency\":null,\"transactionCurrency\":\"EUR\",\"transactionTitle\":null,\"type\":\"1\",\"AC\":null,\"AID\":\"A0000000041010\",\"ATC\":null,\"TSI\":\"8000\",\"TVR\":\"0400000000\"},\"CloudInfo\":{\"Device\":\"1850747855_2\",\"TerminalType\":\"eService\",\"MqttClientId\":\"1bLU\",\"Command\":\"sale\",\"ApkVersion\":\"1.0.86.0629\",\"TerminalModel\":\"PAX_A920Pro\"},\"ResultId\":\"Zvx3hYCS9tXxfAVy\"}}";
            JsonDoc json = JsonDoc.Parse(responseJson);
            /** @var DiamondCloudResponse $parsedResponse */
            ITerminalResponse parsedResponse = service.ParseResponse(responseJson);

            Assert.AreEqual(json.GetValue<string>("CloudTxnId"), parsedResponse.TransactionId);
            Assert.AreEqual(json.Get("PaymentResponse").Get("PaymentResponse").GetValue<string>("authorizationCode"), parsedResponse.AuthorizationCode);
            Assert.AreEqual(posID, parsedResponse.TerminalRefNumber);
            Assert.AreEqual(json.Get("PaymentResponse").Get("CloudInfo").GetValue<string>("Command"), parsedResponse.Command);
        }

        [TestMethod]
        public void SaleACKStatusUrlResponse() {
            DeviceCloudService service = new DeviceCloudService(new DiamondCloudConfig {
                DeviceType = DeviceType.PAX_A920,
                ConnectionMode = ConnectionModes.DIAMOND_CLOUD,
                RequestIdProvider = _requestidProvider,
                Timeout = 15,
                IsvID = "154F070E3E474AB98B00D73ED81AAA93",
                SecretKey = "8003672638",
                Region = Region.EU.ToString(),
                PosID = posID
            });
            string responseJson = "{\"IsvId\":\"154F070E3E474AB98B00D73ED81AAA93\",\"InvoiceId\":\"\",\"CloudTxnId\":\"EXKX7WKV4QX\",\"traceId\":\"\",\"followId\":\"\",\"Device\":\"1850747855_2\",\"PosId\":\"1342641186174645\",\"PaymentResponse\":{\"PaymentResponse\":{\"resultCode\":\"T03\",\"hostMessage\":\"ACKNOWLEDGEEXKX7WKV4QX\",\"transactionId\":\"EXKX7WKV4QX\"},\"CloudInfo\":{\"Device\":\"1850747855_2\",\"TerminalType\":\"eService\",\"MqttClientId\":\"1bLU\",\"Command\":\"sale\",\"ApkVersion\":\"1.0.86.0629\",\"TerminalModel\":\"PAX_A920Pro\"},\"ResultId\":\"mNbmvleC3I63omNK\"}}";
            JsonDoc json = JsonDoc.Parse(responseJson);
            /** @var DiamondCloudResponse $parsedResponse */
            ITerminalResponse parsedResponse = service.ParseResponse(responseJson);

            Assert.AreEqual(json.GetValue<string>("CloudTxnId"), parsedResponse.TransactionId);
            Assert.AreEqual(json.Get("PaymentResponse").Get("CloudInfo").GetValue<string>("Command"), parsedResponse.Command);
            Assert.AreEqual(posID, parsedResponse.TerminalRefNumber);
            Assert.AreEqual(json.Get("PaymentResponse").Get("PaymentResponse").GetValue<string>("resultCode"), parsedResponse.ResponseCode);
            Assert.AreEqual(json.Get("PaymentResponse").Get("PaymentResponse").GetValue<string>("hostMessage"), parsedResponse.ResponseText);
        }

        [TestMethod]
        public void CreditSale_WithoutAmount() {
            bool errorFound = false;
            try {
                _device.Sale()
                    .Execute();
            }
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", e.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void GetTransactionStatus_IdNotFound() {            
            ITerminalReport response = _device.LocalDetailReport()
                .Where(DiamondCloudSearchCriteria.ReferenceNumber, "A49KDND5W3Z")
                .Execute();

            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual(null, response.Status);
        }

        [TestMethod]
        public void GetTransactionStatus_NoId() {
            bool errorFound = false;
            try {
                _device.LocalDetailReport()
                    .Execute();
            }
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("ReferenceNumber cannot be null for this transaction type.", e.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void CreditVoid_WithoutTransactionId() {
            bool errorFound = false;
            try {
                _device.Void()
                    .Execute();
            }
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("TransactionId cannot be null for this transaction type.", e.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void CreditReturn_WithoutAmount() {
            bool errorFound = false;
            try {
                _device.Refund()
                    .Execute();
            }
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", e.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void Authorize_WithoutAmount() {
            bool errorFound = false;
            try {
                _device.Authorize()
                    .Execute();
            }
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", e.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void Capture_WithoutAmount() {
            bool errorFound = false;
            try {
                _device.Capture()
                    .WithTransactionId("BWMNKQK6EB5")
                    .Execute();
            }
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("Amount cannot be null for this transaction type.", e.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void Capture_WithoutTransactionId() {
            bool errorFound = false;
            try {
                _device.Capture(0.2m)
                    .Execute();
            }
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("TransactionId cannot be null for this transaction type.", e.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void AuthIncreasing_WithoutTransactionId() {
            bool errorFound = false;
            try {
                _device.IncreasePreAuth(3)
                    .Execute();
            }
            catch (BuilderException e) {
                errorFound = true;
                Assert.AreEqual("TransactionId cannot be null for this transaction type.", e.Message);
            }
            finally {
                Assert.IsTrue(errorFound);
            }
        }

        private void SetUSConfig() {
            _device = DeviceService.Create(new DiamondCloudConfig {
                DeviceType = DeviceType.PAX_A920,
                ConnectionMode = ConnectionModes.DIAMOND_CLOUD,
                RequestIdProvider = _requestidProvider,
                Timeout = 15,
                IsvID = "154F070E3E474AB98B00D73ED81AAA93",
                SecretKey = "8003672638",
                Region = Region.US.ToString(),
                PosID = posID
            });
        }
    }
}
